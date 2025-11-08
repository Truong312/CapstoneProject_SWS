using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SWS.BusinessObjects.AppSettings;
using SWS.BusinessObjects.Dtos.WhisperDtos;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SWS.Services.Services.WhisperServices;

public interface IWhisperService
{
    Task<WhisperTranscriptionResponse> TranscribeAudioAsync(IFormFile audioFile);
}

public class WhisperService : IWhisperService
{
    private readonly HttpClient _httpClient;
    private readonly WhisperSettings _settings;

    public WhisperService(IHttpClientFactory httpClientFactory, IOptions<WhisperSettings> settings)
    {
        _httpClient = httpClientFactory.CreateClient("whisper");
        _settings = settings.Value;
    }

    public async Task<WhisperTranscriptionResponse> TranscribeAudioAsync(IFormFile audioFile)
    {
        if (audioFile == null || audioFile.Length == 0)
        {
            throw new ArgumentException("Audio file is required", nameof(audioFile));
        }

        using var content = new MultipartFormDataContent();
        using var stream = audioFile.OpenReadStream();
        using var streamContent = new StreamContent(stream);
        
        streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(audioFile.ContentType);
        content.Add(streamContent, "file", audioFile.FileName);

        var response = await _httpClient.PostAsync(
            $"{_settings.BaseUrl}/{_settings.ApiVersion}/audio/transcriptions",
            content
        );

        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadAsStringAsync();
        
        var transcription = JsonSerializer.Deserialize<WhisperTranscriptionResponse>(result, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return transcription ?? throw new InvalidOperationException("Failed to deserialize transcription response");
    }
}

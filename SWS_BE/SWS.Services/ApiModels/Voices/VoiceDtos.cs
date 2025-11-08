using Microsoft.AspNetCore.Http;

namespace Swms.Services.ApiModels.Voices;

public class TranscribeRequest
{
    public IFormFile AudioFile { get; set; } = default!;
    public string Language { get; set; } = "vi";
}

public class TranscribeResponse
{
    public string Text { get; set; } = string.Empty;
}
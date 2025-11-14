using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWS.Services.Services.WhisperServices;
using SWS.Services.ConvertSqlRawServices;
using System;
using System.Threading.Tasks;

namespace SWS.ApiCore.Controllers
{
    [Route("api/[controller]")]
    public class WhisperController : BaseApiController
    {
        private readonly IWhisperService _whisperService;
        private readonly ITextToSqlService _textToSqlService;

        public WhisperController(IWhisperService whisperService, ITextToSqlService textToSqlService)
        {
            _whisperService = whisperService;
            _textToSqlService = textToSqlService;
        }

        /// <summary>
        /// Transcribe audio file to text using Whisper AI
        /// </summary>
        /// <param name="file">Audio file to transcribe (wav, mp3, m4a, etc.)</param>
        /// <returns>Transcribed text with metadata</returns>
        [HttpPost("transcribe")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> TranscribeAudio(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "No audio file provided" });
                }

                var result = await _whisperService.TranscribeAudioAsync(file);
                
                return Ok(new 
                { 
                    success = true,
                    data = result,
                    fileName = file.FileName,
                    fileSize = file.Length
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, 
                    new { message = "Whisper service unavailable", detail = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred while transcribing audio", detail = ex.Message });
            }
        }

        /// <summary>
        /// Upload audio, transcribe it and convert transcription to SQL then execute the SQL.
        /// Returns both transcription and the SQL+rows in one response.
        /// </summary>
        [HttpPost("transcribe-and-query")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> TranscribeAndQuery(IFormFile file, CancellationToken ct)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "No audio file provided" });
                }

                // 1) Transcribe
                var transcription = await _whisperService.TranscribeAudioAsync(file);
                var text = transcription?.Text ?? string.Empty;

                if (string.IsNullOrWhiteSpace(text))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Transcription returned empty text" });
                }

                // 2) Convert text -> SQL and execute
                var queryResult = await _textToSqlService.QueryAsync(text, ct);

                // Return combined payload
                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        transcription,
                        sql = queryResult?.Data?.Sql,
                        rows = queryResult?.Data?.Result
                    }
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable,
                    new { message = "Upstream service unavailable", detail = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while processing the request", detail = ex.Message });
            }
        }
    }
}

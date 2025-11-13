using Microsoft.AspNetCore.Mvc;
using SWS.Services.ApiModels;
using SWS.Services.ApiModels.SqlConverts;
using SWS.Services.ConvertSqlRawServices;

namespace SWS.ApiCore.Controllers
{
    [ApiController]
    [Route("api/text-to-sql")]
    public class TextToSqlController : ControllerBase
    {
        private readonly ITextToSqlService _textToSqlService;

        public TextToSqlController(ITextToSqlService textToSqlService)
        {
            _textToSqlService = textToSqlService;
        }

        /// <summary>
        /// Convert natural language into SQL and execute it
        /// </summary>
        [HttpPost("query")]
        public async Task<ActionResult<ResultModel<SqlQueryResultDto>>> Query([FromBody] SqlQueryRequest req, CancellationToken ct)
        {
            try
            {
                var result = await _textToSqlService.QueryAsync(req.NaturalLanguage, ct);
                // If service returned a result model with StatusCode set and IsSuccess false, map that
                if (result != null && !result.IsSuccess && result.StatusCode >= 400)
                {
                    return StatusCode(result.StatusCode, result);
                }

                return Ok(result);
            }
            catch (SWS.BusinessObjects.Exceptions.AppException ex)
            {
                // Map AppException to structured ResultModel and appropriate HTTP status
                var err = new ResultModel<SqlQueryResultDto>
                {
                    IsSuccess = false,
                    ResponseCode = ex.Code ?? "error",
                    Message = ex.Message,
                    Data = null,
                    StatusCode = ex.StatusCode
                };

                return StatusCode(ex.StatusCode, err);
            }
            catch (Exception ex)
            {
                // Generic fallback
                var err = new ResultModel<SqlQueryResultDto>
                {
                    IsSuccess = false,
                    ResponseCode = "internal_error",
                    Message = ex.Message,
                    Data = null,
                    StatusCode = StatusCodes.Status500InternalServerError
                };

                return StatusCode(StatusCodes.Status500InternalServerError, err);
            }
        }

    }
}
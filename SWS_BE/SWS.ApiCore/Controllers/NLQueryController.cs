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
            var result = await _textToSqlService.QueryAsync(req.NaturalLanguage, ct);
            return Ok(result);
        }

    }
}
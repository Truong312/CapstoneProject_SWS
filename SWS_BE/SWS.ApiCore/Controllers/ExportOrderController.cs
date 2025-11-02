using Microsoft.AspNetCore.Mvc;
using SWS.Services.Services.ExportOrderServices;

namespace SWS.ApiCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExportOrderController : BaseApiController
    {
        private readonly IExportOrderService _exportOrderService;
        public ExportOrderController(IExportOrderService exportOrderService)
        {
            _exportOrderService = exportOrderService;
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAllExportOrder()
        {
            var result = await _exportOrderService.GetAllExportOrdersAsync();
            if (!result.IsSuccess)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        [HttpGet("{id:int}Details")]
        public async Task<IActionResult> GetExportOrderDetails(int id)
        {
            var result = await _exportOrderService.GetExportOrderDetails(id);
            if (!result.IsSuccess)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
    }
}

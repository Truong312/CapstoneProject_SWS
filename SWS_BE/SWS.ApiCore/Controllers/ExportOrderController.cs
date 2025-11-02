using Microsoft.AspNetCore.Mvc;
using SWS.BusinessObjects.Enums;
using SWS.Services.ApiModels.ExportDetailModel;
using SWS.Services.ApiModels.ExportOrderModel;
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
            return Ok(result.Data);
        }

        [HttpGet("by-status")]
        public async Task<IActionResult> GetByStatus([FromQuery] StatusEnums status)
        {
            var result = await _exportOrderService.GetExportOrdersByStatusAsync(status);
            if (!result.IsSuccess)
            {
                return NotFound(result);
            }
            return Ok(result.Data);
        }

        [HttpGet("{id:int}Details")]
        public async Task<IActionResult> GetExportOrderDetails(int id)
        {
            var result = await _exportOrderService.GetExportDetails(id);
            if (!result.IsSuccess)
            {
                return NotFound(result);
            }
            return Ok(result.Data);
        }
        [HttpPost("ExportOder")]
        public async Task<IActionResult> CreateExportOrder([FromBody] CreateExportOrder createExportOrder)
        {
            var result = await _exportOrderService.AddExportOrderAsync(createExportOrder);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("ExportDetail")]
        public async Task<IActionResult> CreateExportDetail(int exportOrderId,[FromBody] CreateExportDetail createExportDetail)
        {
            var result = await _exportOrderService.AddExportDetailAsync(exportOrderId, createExportDetail);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("ExportOder")]
        public async Task<IActionResult> UpdateExportOrder(int exportOrderId,[FromBody] UpdateExportOrder updateExportOrder)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _exportOrderService.UpdateExportOrderAsync(exportOrderId,updateExportOrder);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("ExportDetail")]
        public async Task<IActionResult> UpdateExportDetail(int exportDetailId, [FromBody] UpdateExportDetail updateExportDetail)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _exportOrderService.UpdateExportDetailAsync(exportDetailId, updateExportDetail);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("ExportOder")]
        public async Task<IActionResult> DeleteExportOrder(int exportOrderId)
        {
            var result = await _exportOrderService.DeleteExportOrderAsync(exportOrderId);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("ExportDetail")]
        public async Task<IActionResult> DeleteExportDetail(int exportDetailId)
        {
            var result = await _exportOrderService.DeleteExportDetailAsync(exportDetailId);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}

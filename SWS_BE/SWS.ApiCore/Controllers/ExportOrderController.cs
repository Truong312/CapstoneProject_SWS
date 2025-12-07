using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWS.BusinessObjects.Enums;
using SWS.Services.ApiModels.ExportDetailModel;
using SWS.Services.ApiModels.ExportOrderModel;
using SWS.Services.Services.ExportOrderServices;

namespace SWS.ApiCore.Controllers
{
    [Authorize]
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

        /// <summary>
        /// Lấy danh sách ExportOrder với filter đầy đủ và phân trang
        /// </summary>
        /// <param name="from">Từ ngày (yyyy-MM-dd, nullable)</param>
        /// <param name="to">Đến ngày (yyyy-MM-dd, nullable)</param>
        /// <param name="status">Trạng thái (nullable, so khớp exact: Pending, Completed, Cancelled)</param>
        /// <param name="customerId">Mã khách hàng (nullable)</param>
        /// <param name="createdBy">UserID người tạo (nullable)</param>
        /// <param name="invoiceNumber">Số hóa đơn (nullable, tìm kiếm partial match)</param>
        /// <param name="pageNumber">Số trang (mặc định = 1)</param>
        /// <param name="pageSize">Số lượng items mỗi trang (mặc định = 10, tối đa = 100)</param>
        /// <returns>Danh sách ExportOrder có phân trang</returns>
        [HttpGet("Filter")]
        public async Task<IActionResult> GetExportOrdersWithFilter(
            [FromQuery] string? from,
            [FromQuery] string? to,
            [FromQuery] string? status,
            [FromQuery] int? customerId,
            [FromQuery] int? createdBy,
            [FromQuery] string? invoiceNumber,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            DateOnly? fromDate = TryParseDateOnly(from);
            DateOnly? toDate = TryParseDateOnly(to);

            var result = await _exportOrderService.GetExportOrdersWithFilterAsync(
                fromDate, 
                toDate, 
                status, 
                customerId, 
                createdBy, 
                invoiceNumber,
                pageNumber, 
                pageSize);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(new
            {
                isSuccess = result.IsSuccess,
                statusCode = result.StatusCode,
                message = result.Message,
                data = result.Data
            });
        }

        [HttpGet("by-status")]
        public async Task<IActionResult> GetByStatus([FromQuery] StatusEnums status)
        {
            var result = await _exportOrderService.GetExportOrdersByStatusAsync(status);
            if (!result.IsSuccess)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpGet("{id:int}/Details")]
        public async Task<IActionResult> GetExportOrderDetails(int id)
        {
            var result = await _exportOrderService.GetExportDetails(id);
            if (!result.IsSuccess)
            {
                return NotFound(result);
            }
            return Ok(result);
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
        [HttpGet("Date")]
        public async Task<IActionResult> GetExportOrderByDate(DateOnly startDate,DateOnly endDate)
        {
            var result = await _exportOrderService.GetExportOrderByDate(startDate, endDate);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpPut("Review")]
        [Authorize(Roles ="2")]
        public async Task<IActionResult> ReivewExportOrder(int exportOrderId,StatusEnums status)
        {
            var result = await _exportOrderService.ReviewExportOrder(exportOrderId, status.ToString());
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Helper method để parse string thành DateOnly
        /// </summary>
        private static DateOnly? TryParseDateOnly(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            return DateOnly.TryParse(s, out var d) ? d : null;
        }
    }
}

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SWS.Services.ReturnOrders;

namespace SWS.ApiCore.Controllers
{
    [ApiController]
    [Route("api/returns/orders")]
    // [Authorize] // Bật nếu bạn đã cấu hình JWT và muốn khóa endpoint
    public class ReturnOrdersController : ControllerBase
    {
        private readonly IReturnOrderQueryService _svc;
        public ReturnOrdersController(IReturnOrderQueryService svc) => _svc = svc;

        /// <summary>
        /// Danh sách đơn trả hàng (ReturnOrder) có filter theo ngày, trạng thái, exportOrderId, checkedBy, reviewedBy.
        /// </summary>
        /// <param name="from">yyyy-MM-dd (nullable)</param>
        /// <param name="to">yyyy-MM-dd (nullable)</param>
        /// <param name="status">Trạng thái (nullable, so khớp exact)</param>
        /// <param name="exportOrderId">Mã đơn xuất liên quan (nullable)</param>
        /// <param name="checkedBy">UserID người kiểm (nullable)</param>
        /// <param name="reviewedBy">UserID người duyệt (nullable)</param>
        [HttpGet]
        public async Task<IActionResult> GetList(
            [FromQuery] string? from,
            [FromQuery] string? to,
            [FromQuery] string? status,
            [FromQuery] int? exportOrderId,
            [FromQuery] int? checkedBy,
            [FromQuery] int? reviewedBy)
        {
            DateOnly? fromDate = TryParseDateOnly(from);
            DateOnly? toDate = TryParseDateOnly(to);

            var data = await _svc.GetListAsync(fromDate, toDate, status, exportOrderId, checkedBy, reviewedBy);
            return Ok(new
            {
                isSuccess = true,
                statusCode = 200,
                total = data.Count,
                data
            });
        }

        /// <summary>
        /// Chi tiết một đơn trả hàng (header + lines).
        /// </summary>
        /// <param name="id">ReturnOrderId</param>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetDetail([FromRoute] int id)
        {
            var dto = await _svc.GetDetailAsync(id);
            if (dto == null)
                return NotFound(new { isSuccess = false, statusCode = 404, message = "Return order not found" });

            return Ok(new { isSuccess = true, statusCode = 200, data = dto });
        }

        private static DateOnly? TryParseDateOnly(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            return DateOnly.TryParse(s, out var d) ? d : null;
        }
    }
}

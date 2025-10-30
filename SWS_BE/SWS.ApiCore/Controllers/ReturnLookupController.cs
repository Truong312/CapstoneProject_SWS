using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SWS.Services.ReturnLookups;

namespace SWS.ApiCore.Controllers
{
    [ApiController]
    [Route("api/returns")]
    // [Authorize] // Bật nếu bạn đã cấu hình JWT và muốn khóa endpoint
    public class ReturnLookupController : ControllerBase
    {
        private readonly IReturnLookupService _svc;
        public ReturnLookupController(IReturnLookupService svc) => _svc = svc;

        /// <summary>
        /// Danh sách lý do trả hàng (ReturnReason) + search theo q (ReasonCode/Description).
        /// </summary>
        /// <param name="q">Từ khóa tìm kiếm (nullable)</param>
        [HttpGet("reasons")]
        public async Task<IActionResult> GetReasons([FromQuery] string? q)
        {
            var data = await _svc.GetReasonsAsync(q);
            return Ok(new
            {
                isSuccess = true,
                statusCode = 200,
                total = data.Count,
                data
            });
        }

        /// <summary>
        /// Danh sách DISTINCT trạng thái trả hàng (lấy từ ReturnOrder.Status) + đếm số đơn theo từng trạng thái.
        /// </summary>
        /// <param name="q">Từ khóa tìm kiếm (nullable), so khớp theo substring của Status</param>
        [HttpGet("statuses")]
        public async Task<IActionResult> GetStatuses([FromQuery] string? q)
        {
            var data = await _svc.GetStatusesAsync(q);
            return Ok(new
            {
                isSuccess = true,
                statusCode = 200,
                total = data.Count,
                data
            });
        }
    }
}

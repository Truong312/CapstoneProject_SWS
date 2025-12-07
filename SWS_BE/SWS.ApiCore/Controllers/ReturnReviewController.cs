using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWS.ApiCore.Auth;                 // ClaimUtils
using SWS.BusinessObjects.DTOs;
using SWS.Services.ReturnOrders;

namespace SWS.ApiCore.Controllers
{
    [ApiController]
    [Route("api/returns")]
    public class ReturnReviewController : ControllerBase
    {
        private readonly IReturnOrderReviewService _reviewService;
        private readonly IReturnOrderCommandService _commandService;

        public ReturnReviewController(
            IReturnOrderReviewService reviewService,
            IReturnOrderCommandService commandService)
        {
            _reviewService = reviewService;
            _commandService = commandService;
        }

        /// <summary>
        /// Tạo Return Order (nhân viên tạo phiếu trả hàng).
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "1")] // staff
        public async Task<IActionResult> Create(
            [FromBody] CreateReturnOrderRequest body,
            CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var userId = ClaimUtils.GetUserIdOrThrow(User);

            var result = await _commandService.CreateAsync(userId, body, ct);
            // Nếu sau này bạn có GetDetail thì có thể dùng CreatedAtAction
            return Ok(result);
        }

        /// <summary>
        /// Manager duyệt return order (approve/reject).
        /// </summary>
        [HttpPost("{id:int}/review")]
        [Authorize(Roles = "2")]  // only manager
        public async Task<IActionResult> Review(
            int id,
            [FromBody] ReviewReturnOrderRequest body,
            CancellationToken ct = default)
        {
            if (body == null) return BadRequest("Body is required.");
            body.ReturnOrderId = id;

            var managerId = ClaimUtils.GetUserIdOrThrow(User);
            var result = await _reviewService.ReviewAsync(managerId, body, ct);
            return Ok(result);
        }
    }
}

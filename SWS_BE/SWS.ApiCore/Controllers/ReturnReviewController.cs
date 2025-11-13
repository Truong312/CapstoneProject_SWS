
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
        private readonly IReturnOrderReviewService _service;

        public ReturnReviewController(IReturnOrderReviewService service)
        {
            _service = service;
        }

        /// <summary>
        /// Manager duyệt return order (approve/reject).
        /// </summary>
        [HttpPost("{id:int}/review")]
        [Authorize(Policy = "ManagerOnly")]  
        public async Task<IActionResult> Review(int id, [FromBody] ReviewReturnOrderRequest body)
        {
            if (body == null) return BadRequest("Body is required.");
            body.ReturnOrderId = id;

            var managerId = ClaimUtils.GetUserIdOrThrow(User);
            var result = await _service.ReviewAsync(managerId, body);
            return Ok(result);
        }
    }
}

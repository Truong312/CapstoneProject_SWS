using Microsoft.AspNetCore.Mvc;
using SWS.BusinessObjects.Models;
using SWS.Services.Services.CycleCountServices;

namespace SWS.ApiCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CycleCountController : BaseApiController
    {
        private readonly ICycleCountService _cycleCountService;
        public CycleCountController(ICycleCountService cycleCountService)
        {
            _cycleCountService = cycleCountService;
        }

        [HttpPost("Start")]
        public async Task<IActionResult> StartCycleCountReport(int userId)
        {
            var result = await _cycleCountService.StartCycleCountAsync(userId);
            if (!result.IsSuccess)
            {
                return BadRequest();
            }
            return Ok();
        }

        [HttpPut("{cycleCountDetailId:int}/update")]
        public async Task<IActionResult> UpdateCycleCountRecordedQuantity(int cycleCountDetailId, int recordedQuantity)
        {
            var result = await _cycleCountService.UpdateCountedQuantityAsync(cycleCountDetailId, recordedQuantity);
            if (!result.IsSuccess)
            {
                return BadRequest();
            }
            return Ok();
        }
        [HttpPost("Finalize")]
        public async Task<IActionResult> FinalizeCycleCountAsync(int cycleCountId, int userId)
        {
            var result = await _cycleCountService.FinalizeCycleCountAsync(cycleCountId, userId);
            if (!result.IsSuccess)
            {
                return BadRequest();
            }
            return Ok();
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWS.BusinessObjects.Models;
using SWS.Services.Services.CycleCountServices;

namespace SWS.ApiCore.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CycleCountController : BaseApiController
    {
        private readonly ICycleCountService _cycleCountService;
        public CycleCountController(ICycleCountService cycleCountService)
        {
            _cycleCountService = cycleCountService;
        }
        [Authorize(Roles = "2")]
        [HttpPost("Start")]
        public async Task<IActionResult> StartCycleCountReport()
        {
            var result = await _cycleCountService.StartCycleCountAsync();
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [Authorize(Roles = "2,3")]
        [HttpPut("{cycleCountDetailId:int}/update")]
        public async Task<IActionResult> UpdateCycleCountRecordedQuantity(int cycleCountDetailId, int recordedQuantity)
        {
            var result = await _cycleCountService.UpdateCountedQuantityAsync(cycleCountDetailId, recordedQuantity);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [Authorize(Roles ="2")]
        [HttpPost("Finalize/{cycleCountId:int}")]
        public async Task<IActionResult> FinalizeCycleCountAsync(int cycleCountId)
        {
            var result = await _cycleCountService.FinalizeCycleCountAsync(cycleCountId);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [Authorize(Roles = "2")]
        [HttpPost("Finalize/{cycleCountName}")]
        public async Task<IActionResult> FinalizeCycleCountAsync(string cycleCountName)
        {
            var result = await _cycleCountService.FinalizeCycleCountAsync(cycleCountName);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWS.BusinessObjects.Models;
using SWS.Services.Services.LocationServices;

namespace SWS.ApiCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _service;

        public LocationController(ILocationService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetLocations()
            => Ok(await _service.GetLocations());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLocationDetail(int id)
        {
            var data = await _service.GetLocationDetail(id);
            return data is null ? NotFound() : Ok(data);
        }
        /// <summary>Get product placement</summary>
        [HttpGet("products/{productId}")]
        public async Task<IActionResult> GetProductLocations(int productId)
            => Ok(await _service.GetProductPlacement(productId));

        [HttpPost("assign")]
        public async Task<IActionResult> AssignProduct([FromBody] Inventory request)
            => await _service.AddInventory(request) ? Ok("Assigned!") : BadRequest();

        [HttpPut("update-quantity")]
        public async Task<IActionResult> UpdateQuantity(int inventoryId, int quantity)
            => await _service.UpdateInventory(inventoryId, quantity) ? Ok("Updated") : NotFound();

        [HttpDelete("remove/{inventoryId}")]
        public async Task<IActionResult> RemoveInventory(int inventoryId)
            => await _service.RemoveInventory(inventoryId) ? Ok("Removed!") : NotFound();

        [HttpGet("suggest")]
        public async Task<IActionResult> Suggest(int productId, int required)
        {
            var shelf = await _service.SuggestLocation(productId, required);
            return shelf == null ? NotFound("Không có vị trí phù hợp") : Ok(shelf);
        }

    }
}

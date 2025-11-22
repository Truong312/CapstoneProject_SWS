using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWS.BusinessObjects.Dtos;
using SWS.Services.Services.InventoryServices;

namespace SWS.ApiCore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _service;
        private readonly IInventoryDashboardService _dashboardservice;

        public InventoryController(IInventoryService service, IInventoryDashboardService dashboardservice)
        {
            _service = service;
            _dashboardservice = dashboardservice;
        }

        [HttpGet("status-summary")]
        public async Task<ActionResult<InventoryStatusSummaryDto>> GetStatusSummary()
        {
            var result = await _service.GetInventoryStatusSummaryAsync();
            return Ok(result);
        }


        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var result = await _dashboardservice.GetDashboardAsync();
            return Ok(result);
        }

        [HttpGet("products")]
        public async Task<ActionResult<List<ProductInventoryDto>>> GetAllProductsInventory()
        {
            return Ok(await _service.GetAllProductInventoryAsync());
        }

        [HttpGet("product-inventory")]
        public async Task<ActionResult<List<ProductInventoryDto>>> GetAllProductInventory()
        {
            var result = await _service.GetAllProductInventoryAsync();
            if (result == null || result.Count == 0)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }

}

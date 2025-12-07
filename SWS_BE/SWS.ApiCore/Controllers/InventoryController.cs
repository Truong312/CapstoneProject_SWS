using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWS.BusinessObjects.Dtos;
using SWS.BusinessObjects.Models;
using SWS.Services.ApiModels.InventoryModel;
using SWS.Services.Services.InventoryServices;

namespace SWS.ApiCore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : BaseApiController
    {
        private readonly IInventoryService _inventoryService;
        private readonly IInventoryDashboardService _dashboardservice;
        public InventoryController(IInventoryService service, IInventoryDashboardService dashboardservice)
        {
            _inventoryService = service;
            _dashboardservice = dashboardservice;
        }
        [HttpGet("All")]
        public async Task<IActionResult> GetAllInventories()
        {
            var result = await _inventoryService.GetAllInventoriesAsync();
            if (!result.IsSuccess)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        [HttpGet("{inventoryId:int}")]
        public async Task<IActionResult> GetInventoryById(int inventoryId)
        {
            var result = await _inventoryService.GetInventoryByIdAsync(inventoryId);
            if (!result.IsSuccess)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        [HttpGet("productId/{productId}")]
        public async Task<IActionResult> GetInventoryByProductId(int productId)
        {
            var result = await _inventoryService.GetInventoryByIdAsync(productId);
            if (!result.IsSuccess)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> AddInventory(AddInventory addInventory)
        {
            var result = await _inventoryService.AddInventoryAsync(addInventory);
            if (!result.IsSuccess)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        [HttpPut("inventoryId")]
        public async Task<IActionResult> UpdateInventory(int inventoryId,UpdateInventory updateInventory)
        {
            var result = await _inventoryService.UpdateInventoryAsync(inventoryId,updateInventory);
            if (!result.IsSuccess)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        [HttpDelete("{inventoryId:int}")]
        public async Task<IActionResult> DeleteInventory(int inventoryId)
        {
            var result = await _inventoryService.DeleteInventoryAsync(inventoryId);
            if (!result.IsSuccess)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        [HttpGet("status-summary")]
        public async Task<ActionResult<InventoryStatusSummaryDto>> GetStatusSummary()
        {
            var result = await _inventoryService.GetInventoryStatusSummaryAsync();
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
            return Ok(await _inventoryService.GetAllProductInventoryAsync());
        }

        [HttpGet("product-inventory")]
        public async Task<ActionResult<List<ProductInventoryDto>>> GetAllProductInventory()
        {
            var result = await _inventoryService.GetAllProductInventoryAsync();
            if (result == null || result.Count == 0)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}

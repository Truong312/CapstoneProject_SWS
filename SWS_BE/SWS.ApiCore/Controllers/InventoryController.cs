using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
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
    }
}

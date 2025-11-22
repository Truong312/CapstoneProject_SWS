using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SWS.Services.Services.WarehouseLayoutServices;

namespace SWS.ApiCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseLayoutController : BaseApiController
    {
        private readonly IWarehouseLayoutService _warehouseLayoutService;

        public WarehouseLayoutController(IWarehouseLayoutService warehouseLayoutService)
        {
            _warehouseLayoutService = warehouseLayoutService;
        }

        /// <summary>
        /// Lấy layout kho 2D theo kệ &/hoặc theo sản phẩm.
        /// GET: api/warehouselayout?shelfId=A1&productId=10
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetLayout([FromQuery] string? shelfId, [FromQuery] int? productId)
        {
            var result = await _warehouseLayoutService.GetLayoutAsync(shelfId, productId);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result);

            return Ok(result.Data);
        }
    }
}

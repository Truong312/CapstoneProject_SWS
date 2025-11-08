using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWS.Services.ApiModels.Commons;
using SWS.Services.ApiModels.ProductModel;
using SWS.Services.Services.ProductServices;

namespace SWS.ApiCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : BaseApiController
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/product
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var result = await _productService.GetAllProductsAsync();
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result.Data);
        }

        // GET: api/product/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var result = await _productService.GetProductByIdAsync(id);
            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result.Data);
        }

        // POST: api/product
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _productService.AddProductAsync(request);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result.Data);
        }

        // PUT: api/product/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _productService.UpdateProductAsync(id, request);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result.Data);
        }

        // DELETE: api/product/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result.Data);
        }

        // GET: api/product/near-expired
        [HttpGet("near-expired")]
        public async Task<IActionResult> NearExpiredProducts()
        {
            var result = await _productService.GetNearExpiredProductsAsync();
            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result.Data);
        }

        // GET: api/product/expired
        [HttpGet("expired")]
        public async Task<IActionResult> ExpiredProducts()
        {
            var result = await _productService.GetExpiredProductsAsync();
            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result.Data);
        }

        // GET: api/product/low-stock
        [HttpGet("low-stock")]
        public async Task<IActionResult> LowStockProducts()
        {
            var result = await _productService.GetLowStockProductsAsync();
            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result.Data);
        }

        // GET: api/product/search?text=abc
        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts([FromQuery] string text)
        {
            var result = await _productService.SearchProductsAsync(text);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result.Data);
        }

        // GET: api/product/paged?page=1&pageSize=20&q=keyword
        [HttpGet("paged")]
        public async Task<IActionResult> GetProductsPaged([FromQuery] PagedRequestDto req)
        {
            var result = await _productService.GetProductsPagedAsync(req);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result.Data);
        }

    }
}

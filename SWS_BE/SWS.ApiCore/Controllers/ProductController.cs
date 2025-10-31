using Microsoft.AspNetCore.Mvc;
using SWS.BusinessObjects.Dtos.Product;
using SWS.Repositories.Repositories.ProductRepo;
using SWS.Services.Services.ProductServices;

namespace SWS.ApiCore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : BaseApiController
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductService _productService;
        public ProductController(IProductRepository productRepository, IProductService productService)
        {
            _productRepository = productRepository;
            _productService = productService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] string? filter, [FromQuery] string? sortBy, [FromQuery] bool ascending = true)
        {
            var products = await _productRepository.GetAllAsync(filter, sortBy, ascending);
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }
            return Ok(product);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            var product = await _productService.CreateProductAsync(dto);
            return Ok();
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] UpdateProductDto dto)
        {
            var product = await _productService.UpdateProductAsync(dto);
            return Ok();
        }
    }
}

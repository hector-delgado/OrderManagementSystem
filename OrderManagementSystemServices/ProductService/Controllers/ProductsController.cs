using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ProductService.Services;
using ProductService.Models;

namespace CustomerService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsService _productsService;

        public ProductsController(IProductsService productsService)
        {
            _productsService = productsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var products = await _productsService.GetAllProducts();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error. Please try again later. Error: {ex.Message}, {ex?.InnerException?.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var product = await _productsService.GetProductById(id);
                if (product == null)
                {
                    return NotFound();
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                //_logger.LogError($"An error occurred while retrieving the customer. Error: {ex.Message}");

                return StatusCode(500, $"Internal Server Error. Please try again later. Error: {ex.Message}, {ex?.InnerException?.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto product)
        {
            try
            {
                if (product == null)
                {
                    return BadRequest();
                }

                var createdProduct = await _productsService.CreateProduct(product);
                if (createdProduct == null)
                {
                    return BadRequest();
                }
                return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.Id }, createdProduct);
            }
            catch (Exception ex)
            {
                //_logger.LogError($"An error occurred while retrieving the customer. Error: {ex.Message}");

                return StatusCode(500, $"Internal Server Error. Please try again later. Error: {ex.Message}, {ex?.InnerException?.Message}");
            }
        }
    }
}
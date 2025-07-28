using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ProductService.Services;
using ProductService.Models;

namespace ProductService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsService _productsService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductsService productsService, ILogger<ProductsController> logger)
        {
            _productsService = productsService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                _logger.LogInformation("GetAllProducts has been called.");
                var products = await _productsService.GetAllProducts();

                _logger.LogInformation("GetAllProducts retrieved successfully.");
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while retrieving products. Error: {ex.Message}");
                return StatusCode(500, $"Internal Server Error. Please try again later. Error: {ex.Message}, {ex?.InnerException?.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                _logger.LogInformation("GetProductById has been called with ID: {Id}", id);

                var product = await _productsService.GetProductById(id);
                if (product == null)
                {
                    _logger.LogWarning("GetProductById did not find a product with ID: {Id}", id);
                    return NotFound();
                }
                _logger.LogInformation("GetProductById retrieved successfully for ID: {Id}", id);
                return Ok(product);
            }
            catch (Exception ex)
            {
                
                _logger.LogError($"An error occurred while retrieving the product with ID {id}. Error: {ex.Message}");
                return StatusCode(500, $"Internal Server Error. Please try again later. Error: {ex.Message}, {ex?.InnerException?.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto product)
        {
            try
            {
                _logger.LogInformation("CreateProduct has been called.");

                if (product == null)
                {
                    return BadRequest();
                }

                var createdProduct = await _productsService.CreateProduct(product);
                if (createdProduct == null)
                {
                    return BadRequest();
                }

                _logger.LogInformation("CreateProduct created successfully with ID: {Id}", createdProduct.Id);

                return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.Id }, createdProduct);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while creating the product. Error: {ex.Message}");
                return StatusCode(500, $"Internal Server Error. Please try again later. Error: {ex.Message}, {ex?.InnerException?.Message}");
            }
        }
    }
}
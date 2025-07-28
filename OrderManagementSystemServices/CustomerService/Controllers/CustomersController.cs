using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CustomerService.Services;
using CustomerService.Models;
using System.Text.Json;

namespace CustomerService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomersService _customersService;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(ICustomersService customersService, ILogger<CustomersController> logger)
        {
            _customersService = customersService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
            try
            {
                _logger.LogInformation("GetAllCustomers has been called.");

                var customers = await _customersService.GetAllCustomers();

                _logger.LogInformation("GetAllCustomers retrieved successfully.");

                return Ok(customers);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while retrieving customers. Error: {ex.Message}");
                return StatusCode(500, "Internal Server Error. Please try again later. Error: {ex.Message}, {ex?.InnerException?.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            try
            {
                _logger.LogInformation("GetCustomerById has been called.");

                var customer = await _customersService.GetCustomerById(id);
                if (customer == null)
                {
                    return NotFound();
                }

                _logger.LogInformation($"GetCustomerById with ID {id} retrieved successfully.");

                return Ok(customer);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while retrieving the customer. Error: {ex.Message}");

                return StatusCode(500, "Internal Server Error. Please try again later. Error: {ex.Message}, {ex?.InnerException?.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerDto customer)
        {
            _logger.LogInformation($"CreateCustomer has been called. request: {JsonSerializer.Serialize(customer)}");

            try
            {
                if (customer == null)
                {
                    return BadRequest();
                }
                var createdCustomer = await _customersService.CreateCustomer(customer);
                if (createdCustomer == null)
                {
                    _logger.LogWarning($"CreateCustomer returned bad request. request: {JsonSerializer.Serialize(customer)}");
                    
                    return BadRequest();
                }

                _logger.LogInformation($"CreateCustomer for email {customer.Email} created successfully.");

                return CreatedAtAction(nameof(GetCustomerById), new { id = createdCustomer.Id }, createdCustomer);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while creating the customer. Error: {ex.Message}");
                return StatusCode(500, "Internal Server Error. Please try again later. Error: {ex.Message}, {ex?.InnerException?.Message}");
            }
        }
    }
}
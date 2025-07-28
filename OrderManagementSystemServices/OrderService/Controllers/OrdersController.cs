using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OrderService.Models;
using OrderService.Services;
using System.Text.Json;

namespace OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersService _ordersService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrdersService ordersService, ILogger<OrdersController> logger)
        {
            _ordersService = ordersService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                _logger.LogInformation("GetAllOrders has been called");

                var orders = await _ordersService.GetAllOrders();

                _logger.LogInformation("GetAllOrders has been completed successfully");

                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while retrieving all orders. Error: {ex.Message}");

                return StatusCode(500, $"Internal Server Error. Please try again later. Error: {ex.Message}, {ex?.InnerException?.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            try
            {
                _logger.LogInformation($"GetOrderById has been called. request: {id}");

                var order = await _ordersService.GetOrderById(id);
                if (order == null)
                {
                    return NotFound();
                }

                _logger.LogInformation($"GetOrderById has been called. request: {id}");

                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while retrieving the customer. Error: {ex.Message}");

                return StatusCode(500, $"Internal Server Error. Please try again later. Error: {ex.Message}, {ex.InnerException?.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto orderDto)
        {
            try
            {
                _logger.LogInformation($"CreateOrder has been called. request: {JsonSerializer.Serialize(orderDto)}");

                if (orderDto == null)
                {
                    return BadRequest();
                }

                var result = await _ordersService.CreateOrder(orderDto);
                if (result == null)
                {
                    return BadRequest("Order creation failed. Please check the input data.");
                }
                else if (!result.IsStockAvailable)
                {
                    return BadRequest(result.ErrorMessage ?? "Stock is not available for the requested product.");
                }
                _logger.LogInformation($"CreateOrder has been finished. request: {JsonSerializer.Serialize(orderDto)}");

                // Use the actual order ID from the result
                return CreatedAtAction(
                    actionName: nameof(GetOrderById),
                    routeValues: new { id = result.Order.Id },
                    value: result.Order);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while creating the order. Error: {ex.Message}");

                return StatusCode(500, $"Internal Server Error. Please try again later. Error: {ex.Message}, {ex?.InnerException?.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] UpdateOrderDto orderDto)
        {
            _logger.LogInformation($"UpdateOrder has been called. request: {id}");

            if (orderDto == null)
            {
                return BadRequest();
            }

            var updatedOrder = await _ordersService.UpdateOrder(id, orderDto);
            if (updatedOrder == null)
            {
                return NotFound();
            }

            _logger.LogInformation($"UpdateOrder has been called. request: {id}");

            return Ok(updatedOrder);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            _logger.LogInformation($"DeleteOrder has been called. request: {id}");

            var deleted = await _ordersService.DeleteOrder(id);
            if (!deleted)
            {
                return NotFound();
            }

            _logger.LogInformation($"DeleteOrder has been called. request: {id}");
            
            return NoContent();
        }
    }
}

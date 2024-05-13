using Microsoft.AspNetCore.Mvc;
using SampleAPI.Entities;
using SampleAPI.Repositories;
using SampleAPI.Service;

namespace SampleAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderService _orderService;

        public OrdersController(IOrderRepository orderRepository, IOrderService orderService)
        {
            _orderRepository = orderRepository;
            _orderService = orderService;
        }

        [HttpGet("recent")]
        public async Task<ActionResult<IEnumerable<Order>>> GetRecentOrders()
        {
            try
            {
                var recentOrders = await _orderRepository.GetRecentOrdersAsync();
                return Ok(recentOrders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("submit")]
        public async Task<ActionResult> SubmitOrder(Order order)
        {
            try
            {
                if (order == null)
                    return BadRequest("Order data is missing");

                if (string.IsNullOrEmpty(order.Name) || string.IsNullOrEmpty(order.Description))
                    return BadRequest("Order name and description are required");

                await _orderService.AddOrderAsync(order);
                return CreatedAtAction(nameof(GetRecentOrders), new { id = order.Id }, order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("recent/excludenonbusinessdays")]
        public async Task<ActionResult<IEnumerable<Order>>> GetRecentOrdersExcludingNonBusinessDays(int days)
        {
            try
            {
                var orders = await _orderService.GetRecentOrdersExcludingNonBusinessDaysAsync(days);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

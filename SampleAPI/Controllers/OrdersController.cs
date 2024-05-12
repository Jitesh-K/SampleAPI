using Microsoft.AspNetCore.Mvc;
using SampleAPI.Entities;
using SampleAPI.Repositories;
using SampleAPI.Requests;

namespace SampleAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        // Add more dependencies as needed.

        public OrdersController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
 
        [HttpGet("")] // TODO: Change route, if needed.
        [ProducesResponseType(StatusCodes.Status200OK)] // TODO: Add all response types
        public async Task<ActionResult<List<Order>>> GetOrders()
        {
            throw new NotImplementedException();
        }

        /// TODO: Add an endpoint to allow users to create an order using <see cref="CreateOrderRequest"/>.
    }
}

using Microsoft.AspNetCore.Mvc;
using Moq;
using SampleAPI.Controllers;
using SampleAPI.Entities;
using SampleAPI.Repositories;
using SampleAPI.Service;

namespace SampleAPI.Tests.Controllers
{
    public class OrdersControllerTests
    {
        private readonly Mock<IOrderRepository> _mockRepository;
        private readonly Mock<IOrderService>  _mockService;
        private readonly OrdersController _controller;

        public OrdersControllerTests()
        {
            _mockRepository = new Mock<IOrderRepository>();
            _mockService = new Mock<IOrderService>();
            _controller = new OrdersController(_mockRepository.Object,_mockService.Object);
        }

        [Fact]
        public async Task GetRecentOrders_ReturnsOkResult_WithRecentOrders()
        {
            // Arrange
            var expectedOrders = new List<Order> { new Order { Name = "Order1", Description = "Description1" } };
            _mockRepository.Setup(repo => repo.GetRecentOrdersAsync()).ReturnsAsync(expectedOrders);

            // Act
            var result = await _controller.GetRecentOrders();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var orders = Assert.IsAssignableFrom<IEnumerable<Order>>(okResult.Value);
            Assert.Equal(expectedOrders, orders);
        }

        [Fact]
        public async Task GetRecentOrders_ReturnsStatusCode500_WhenRepositoryThrowsException()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetRecentOrdersAsync()).ThrowsAsync(new Exception("Repository error"));

            // Act
            var result = await _controller.GetRecentOrders();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task SubmitOrder_ReturnsCreatedAtActionResult_WithCreatedOrder()
        {
            // Arrange
            var order = new Order { Name = "Test Order", Description = "Test Description" };
            _mockService.Setup(repo => repo.AddOrderAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.SubmitOrder(order);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_controller.GetRecentOrders), createdAtActionResult.ActionName);
            Assert.Equal(order, createdAtActionResult.Value);
        }

        [Fact]
        public async Task SubmitOrder_ReturnsBadRequest_WhenOrderDataIsMissing()
        {
            // Act
            var result = await _controller.SubmitOrder(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Order data is missing", badRequestResult.Value);
        }

        [Fact]
        public async Task SubmitOrder_ReturnsBadRequest_WhenOrderNameOrDescriptionIsMissing()
        {
            // Arrange
            var orderWithMissingData = new Order();

            // Act
            var result = await _controller.SubmitOrder(orderWithMissingData);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Order name and description are required", badRequestResult.Value);
        }

        [Fact]
        public async Task SubmitOrder_ReturnsStatusCode500_WhenRepositoryThrowsException()
        {
            // Arrange
            var order = new Order { Name = "test", Description = "test" };
            _mockService.Setup(repo => repo.AddOrderAsync(It.IsAny<Order>())).ThrowsAsync(new Exception("Repository error"));

            // Act
            var result = await _controller.SubmitOrder(order);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task GetRecentOrdersExcludingNonBusinessDays_ReturnsOkResult_WithRecentOrders()
        {
            // Arrange
            int days = 7; // Number of days for recent orders
            var expectedOrders = new List<Order> { new Order { Name = "Order1", Description = "Description1" } };
            _mockService.Setup(service => service.GetRecentOrdersExcludingNonBusinessDaysAsync(days)).ReturnsAsync(expectedOrders);

            // Act
            var result = await _controller.GetRecentOrdersExcludingNonBusinessDays(days);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var orders = Assert.IsAssignableFrom<IEnumerable<Order>>(okResult.Value);
            Assert.Equal(expectedOrders, orders);
        }

        [Fact]
        public async Task GetRecentOrdersExcludingNonBusinessDays_ReturnsStatusCode500_WhenServiceThrowsException()
        {
            // Arrange
            int days = 7; 
            _mockService.Setup(service => service.GetRecentOrdersExcludingNonBusinessDaysAsync(days)).ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _controller.GetRecentOrdersExcludingNonBusinessDays(days);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using SampleAPI.Entities;
using SampleAPI.Repositories;
using SampleAPI.Service;
using Xunit;

namespace SampleAPI.Tests.Service
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _mockRepository;
        private readonly Mock<ILogger<OrderService>> _mockLogger;
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _mockRepository = new Mock<IOrderRepository>();
            _mockLogger = new Mock<ILogger<OrderService>>();
            _orderService = new OrderService(_mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task AddOrderAsync_SuccessfullyAddsOrder()
        {
            // Arrange
            var order = new Order { Name = "Test Order", Description = "Test Description" };

            // Act
            await _orderService.AddOrderAsync(order);

            // Assert
            _mockRepository.Verify(repo => repo.AddOrderAsync(order), Times.Once);
        }

        [Fact]
        public async Task AddOrderAsync_ThrowsArgumentNullException_WhenOrderIsNull()
        {
            // Arrange
            Order order = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _orderService.AddOrderAsync(order));
        }

        [Fact]
        public async Task GetRecentOrdersExcludingNonBusinessDaysAsync_ReturnsExpectedOrders()
        {
            // Arrange
            var startDate = new DateTime(2024, 5, 1);
            var endDate = new DateTime(2024, 5, 10);
            var expectedOrders = new List<Order> { new Order { Name = "Order1", Description = "Description1" } };

            _mockRepository.Setup(repo => repo.GetRecentOrdersExcludingNonBusinessDaysAsync(It.IsAny<DateTime>(),It.IsAny<DateTime>()))
                           .ReturnsAsync(expectedOrders);

            // Act
            var result = await _orderService.GetRecentOrdersExcludingNonBusinessDaysAsync(10);

            // Assert
            Assert.Equal(expectedOrders, result);
        }

        [Fact]
        public async Task GetRecentOrdersExcludingNonBusinessDaysAsync_ThrowsException_WhenRepositoryThrowsException()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetRecentOrdersExcludingNonBusinessDaysAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                           .ThrowsAsync(new Exception("Repository error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _orderService.GetRecentOrdersExcludingNonBusinessDaysAsync(10));
        }

        [Fact]
        public async Task GetRecentOrdersExcludingNonBusinessDaysAsync_ReturnsEmptyList_WhenNoOrdersFound()
        {
            // Arrange
            var startDate = new DateTime(2024, 5, 1);
            var endDate = new DateTime(2024, 5, 10);
            var expectedOrders = Enumerable.Empty<Order>();

            _mockRepository.Setup(repo => repo.GetRecentOrdersExcludingNonBusinessDaysAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                           .ReturnsAsync(expectedOrders);

            // Act
            var result = await _orderService.GetRecentOrdersExcludingNonBusinessDaysAsync(10);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetRecentOrdersExcludingNonBusinessDaysAsync_ReturnsOrders_WhenStartDateIsLaterThanEndDate()
        {
            // Arrange
            var startDate = new DateTime(2024, 5, 10);
            var endDate = new DateTime(2024, 5, 1);
            var expectedOrders = new List<Order> { new Order { Name = "Order1", Description = "Description1" } };

            _mockRepository.Setup(repo => repo.GetRecentOrdersExcludingNonBusinessDaysAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                           .ReturnsAsync(expectedOrders);

            // Act
            var result = await _orderService.GetRecentOrdersExcludingNonBusinessDaysAsync(10);

            // Assert
            Assert.Equal(expectedOrders, result);
        }
    }
}

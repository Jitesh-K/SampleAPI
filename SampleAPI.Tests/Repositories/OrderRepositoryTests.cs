using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using SampleAPI.Entities;
using SampleAPI.Repositories;
using Xunit;

namespace SampleAPI.Tests.Repositories
{
    public class OrderRepositoryTests
    {
        private readonly SampleApiDbContext _dbContext;
        private readonly ILogger<OrderRepository> _logger;

        public OrderRepositoryTests()
        {
            var serviceProvider = new ServiceCollection()
                .AddDbContext<SampleApiDbContext>(options => options.UseInMemoryDatabase(databaseName: "SampleDB"))
                .AddLogging()
                .BuildServiceProvider();

            _dbContext = serviceProvider.GetRequiredService<SampleApiDbContext>();
            _logger = serviceProvider.GetRequiredService<ILogger<OrderRepository>>();
        }

        [Fact]
        public async Task GetRecentOrdersAsync_ReturnsRecentOrders()
        {
            // Arrange
            await SeedTestData();

            var repository = new OrderRepository(_dbContext, _logger);

            // Act
            var recentOrders = await repository.GetRecentOrdersAsync();

            // Assert
            Assert.NotNull(recentOrders);
            Assert.Equal(2, recentOrders.Count());
            Assert.All(recentOrders, o => Assert.False(o.Deleted));
            Assert.All(recentOrders, o => Assert.True(o.EntryDate > DateTime.UtcNow.AddDays(-1)));
        }

        private async Task SeedTestData()
        {
            var orders = new List<Order>
            {
                new Order { EntryDate = DateTime.UtcNow, Deleted = false },
                new Order { EntryDate = DateTime.UtcNow, Deleted = false },
                new Order { EntryDate = DateTime.UtcNow, Deleted = true },
                new Order { EntryDate = DateTime.UtcNow.AddDays(-1), Deleted = false },
                new Order { EntryDate = DateTime.UtcNow.AddDays(-2), Deleted = true }
            };

            await _dbContext.Orders.AddRangeAsync(orders);
            await _dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task AddOrderAsync_AddsOrderToDbContext()
        {
            // Arrange
            var mockDbSet = new Mock<DbSet<Order>>();
            var mockContext = new Mock<SampleApiDbContext>();
            mockContext.Setup(c => c.Orders).Returns(mockDbSet.Object);

            var repository = new OrderRepository(mockContext.Object, _logger);

            // Act
            await repository.AddOrderAsync(new Order());

            // Assert
            mockDbSet.Verify(m => m.Add(It.IsAny<Order>()), Times.Once);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using SampleAPI.Entities;

namespace SampleAPI.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly SampleApiDbContext _dbContext;
        private readonly ILogger<OrderRepository> _logger;

        public OrderRepository(SampleApiDbContext dbContext, ILogger<OrderRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IEnumerable<Order>> GetRecentOrdersAsync()
        {
            try
            {
                var oneDayAgo = DateTime.UtcNow.AddDays(-1);
                var recentOrders = await _dbContext.Orders
                    .Where(o => o.EntryDate > oneDayAgo && !o.Deleted)
                    .OrderByDescending(o => o.EntryDate)
                    .ToListAsync();

                return recentOrders;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OrderRepository.GetRecentOrdersAsync");
                throw;
            }
        }

        public async Task AddOrderAsync(Order order)
        {
            try
            {
                _dbContext.Orders.Add(order);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ROrderRepository.AddOrderAsync");
                throw;
            }
        }

        public async Task<IEnumerable<Order>> GetRecentOrdersExcludingNonBusinessDaysAsync(DateTime startDate, DateTime adjustedEndDate)
        {
            try
            {
                var recentOrders = await _dbContext.Orders
                        .Where(o => o.EntryDate >= startDate && o.EntryDate <= adjustedEndDate && !o.Deleted)
                        .OrderByDescending(o => o.EntryDate)
                        .ToListAsync();
                return recentOrders;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OrderRepository.AddOrderAsync");
                throw;
            }
        }
    }
}

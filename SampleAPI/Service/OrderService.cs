using SampleAPI.Entities;
using SampleAPI.Repositories;

namespace SampleAPI.Service
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<OrderService> _logger;
        public OrderService(IOrderRepository orderRepository, ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }
        public async Task AddOrderAsync(Order order)
        {
            try
            {
                if (order == null)
                    throw new ArgumentNullException(nameof(order), "Order data is missing");
                await _orderRepository.AddOrderAsync(order);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "Error occured in OrderService.AddOrderAsync");
                throw;
            }
        }

        public Task<IEnumerable<Order>> GetRecentOrdersExcludingNonBusinessDaysAsync(int days)
        {
            try
            {
                // Calculate start and end dates
                var endDate = DateTime.UtcNow;
                var startDate = endDate.AddDays(-days);

                // Adjust end date to include orders from the next working days
                var totalDays = (int)(endDate - startDate).TotalDays;
                var remainingDays = days;
                var adjustedEndDate = endDate;

                while (remainingDays > 0)
                {
                    adjustedEndDate = adjustedEndDate.AddDays(1);
                    if (adjustedEndDate.DayOfWeek != DayOfWeek.Saturday && adjustedEndDate.DayOfWeek != DayOfWeek.Sunday)
                    {
                        remainingDays--;
                    }
                }

                // Query for orders within the adjusted date range
                return _orderRepository.GetRecentOrdersExcludingNonBusinessDaysAsync(startDate, adjustedEndDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured in OrderService.GetRecentOrdersExcludingNonBusinessDaysAsync");
                throw;
            }
        }
    }
}

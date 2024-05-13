using SampleAPI.Entities;

namespace SampleAPI.Service
{
    public interface IOrderService
    {
        Task AddOrderAsync(Order order);
        Task<IEnumerable<Order>> GetRecentOrdersExcludingNonBusinessDaysAsync(int days);
    }
}

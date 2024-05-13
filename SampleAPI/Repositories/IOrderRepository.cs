using System.Collections.Generic;
using System.Threading.Tasks;
using SampleAPI.Entities;

namespace SampleAPI.Repositories
{
    /// <summary>
    /// Interface for interacting with orders.
    /// </summary>
    public interface IOrderRepository
    {
        /// <summary>
        /// Adds a new order asynchronously.
        /// </summary>
        /// <param name="order">The order to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddOrderAsync(Order order);

        /// <summary>
        /// Retrieves recent orders asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation that returns a collection of recent orders.</returns>
        Task<IEnumerable<Order>> GetRecentOrdersAsync();
        Task<IEnumerable<Order>> GetRecentOrdersExcludingNonBusinessDaysAsync(DateTime startDate, DateTime adjustedEndDate);
    }
}

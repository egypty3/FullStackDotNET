using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

/// <summary>
/// Specialized repository interface for Order entities.
/// Extends the generic IRepository with Order-specific query methods.
/// 
/// Provides queries essential for the order management workflow: retrieving a customer's order history
/// and loading orders with their associated line items (eager loading).
/// </summary>
public interface IOrderRepository : IRepository<Order>
{
    /// <summary>
    /// Retrieves all orders placed by a specific customer.
    /// Used for the "My Orders" / order history page in the customer-facing UI.
    /// </summary>
    /// <param name="customerId">The customer's ID to filter orders by.</param>
    /// <param name="cancellationToken">Token to cancel the async operation.</param>
    /// <returns>A read-only list of the customer's orders.</returns>
    Task<IReadOnlyList<Order>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an order by ID with its OrderItems eagerly loaded (included in the query).
    /// Without this, accessing order.Items would require a separate database query (lazy loading)
    /// or would return an empty collection (if lazy loading is disabled).
    /// Used for order detail views and order processing workflows.
    /// </summary>
    /// <param name="orderId">The order's unique identifier.</param>
    /// <param name="cancellationToken">Token to cancel the async operation.</param>
    /// <returns>The order with its items loaded, or null if not found.</returns>
    Task<Order?> GetWithItemsAsync(Guid orderId, CancellationToken cancellationToken = default);
}

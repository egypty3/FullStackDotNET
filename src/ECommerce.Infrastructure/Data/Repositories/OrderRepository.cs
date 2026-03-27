using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Data.Repositories;

/// <summary>
/// Repository for <see cref="Order"/> entities. Provides queries to retrieve orders
/// with their line items (<see cref="OrderItem"/>) eagerly loaded, and to filter
/// orders by customer.
/// </summary>
public class OrderRepository : Repository<Order>, IOrderRepository
{
    /// <inheritdoc />
    public OrderRepository(ApplicationDbContext context) : base(context) { }

    /// <summary>
    /// Retrieves all orders for a given customer, including line items, ordered by newest first.
    /// </summary>
    /// <param name="customerId">The customer identifier to filter by.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    public async Task<IReadOnlyList<Order>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default)
        => await _dbSet
            .Include(o => o.Items)
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);

    /// <summary>
    /// Retrieves a single order by its ID with all line items eagerly loaded.
    /// </summary>
    /// <param name="orderId">The unique identifier of the order.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    public async Task<Order?> GetWithItemsAsync(Guid orderId, CancellationToken cancellationToken = default)
        => await _dbSet
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);
}

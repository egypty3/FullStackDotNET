using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using ECommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Data.Repositories;

/// <summary>
/// Repository for <see cref="Payment"/> entities. Overrides the base <c>GetByIdAsync</c> to
/// eager-load the related <see cref="Order"/> and provides look-ups by order ID,
/// external transaction ID, and payment status.
/// </summary>
public class PaymentRepository : Repository<Payment>, IPaymentRepository
{
    /// <inheritdoc />
    public PaymentRepository(ApplicationDbContext context) : base(context) { }

    /// <summary>
    /// Retrieves a payment by ID with its related <see cref="Order"/> included.
    /// </summary>
    public override async Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbSet.Include(p => p.Order).FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    /// <summary>
    /// Finds the payment associated with the given <paramref name="orderId"/>,
    /// including the related order details.
    /// </summary>
    public async Task<Payment?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
        => await _dbSet.Include(p => p.Order).FirstOrDefaultAsync(p => p.OrderId == orderId, cancellationToken);

    /// <summary>
    /// Finds a payment by its external gateway <paramref name="transactionId"/>.
    /// </summary>
    public async Task<Payment?> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken = default)
        => await _dbSet.FirstOrDefaultAsync(p => p.TransactionId == transactionId, cancellationToken);

    /// <summary>
    /// Returns all payments with the given <paramref name="status"/>, ordered by newest first.
    /// </summary>
    public async Task<IReadOnlyList<Payment>> GetByStatusAsync(PaymentStatus status, CancellationToken cancellationToken = default)
        => await _dbSet.Where(p => p.Status == status).OrderByDescending(p => p.CreatedAt).ToListAsync(cancellationToken);
}

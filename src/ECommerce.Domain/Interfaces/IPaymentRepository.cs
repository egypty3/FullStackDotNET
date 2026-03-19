using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<Payment?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Payment?> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Payment>> GetByStatusAsync(Enums.PaymentStatus status, CancellationToken cancellationToken = default);
}

using ECommerce.Domain.Common;
using ECommerce.Domain.Enums;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Domain.Entities;

public class Payment : BaseEntity
{
    public Guid OrderId { get; private set; }
    public Order Order { get; private set; } = null!;
    public Money Amount { get; private set; } = new(0);
    public PaymentMethod Method { get; private set; }
    public string? TransactionId { get; private set; }
    public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;
    public DateTime? PaidAt { get; private set; }
    public string? FailureReason { get; private set; }

    private Payment() { }

    public static Payment Create(Guid orderId, decimal amount, PaymentMethod method, string? transactionId = null)
    {
        if (amount <= 0)
            throw new ArgumentException("Payment amount must be positive.");

        return new Payment
        {
            OrderId = orderId,
            Amount = new Money(amount),
            Method = method,
            TransactionId = transactionId
        };
    }

    public void MarkAsCompleted(string transactionId)
    {
        if (string.IsNullOrWhiteSpace(transactionId))
            throw new ArgumentException("Transaction ID is required.");

        Status = PaymentStatus.Completed;
        TransactionId = transactionId;
        PaidAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsFailed(string reason)
    {
        Status = PaymentStatus.Failed;
        FailureReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsRefunded()
    {
        Status = PaymentStatus.Refunded;
        UpdatedAt = DateTime.UtcNow;
    }
}

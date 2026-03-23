using ECommerce.Domain.Common;
using ECommerce.Domain.Enums;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Domain.Entities;

/// <summary>
/// Represents a payment transaction associated with an order.
/// Tracks the payment lifecycle from creation through completion, failure, or refund.
/// 
/// Payment Lifecycle:
///   Pending -> Completed (success)  |  Failed (declined/error)
///   Completed -> Refunded (reversal)
/// 
/// Key design aspects:
/// - Each Payment is linked to exactly one Order via the OrderId foreign key.
/// - The Order navigation property provides access to the related order entity (loaded by EF Core).
/// - Stores the external TransactionId from the payment gateway for reconciliation.
/// - FailureReason captures why a payment failed (e.g., "Insufficient funds").
/// - Uses the Money value object for the amount to maintain currency consistency.
/// 
/// Inherits Id, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy from BaseEntity.
/// </summary>
public class Payment : BaseEntity
{
    /// <summary>Foreign key to the Order this payment is for.</summary>
    public Guid OrderId { get; private set; }

    /// <summary>
    /// Navigation property to the associated Order entity.
    /// Populated by EF Core when the order is included in a query (eager/lazy loading).
    /// The null-forgiving operator (null!) indicates EF Core guarantees this won't be null after loading.
    /// </summary>
    public Order Order { get; private set; } = null!;

    /// <summary>The payment amount stored as a Money value object (includes currency).</summary>
    public Money Amount { get; private set; } = new(0);

    /// <summary>The payment method chosen by the customer (e.g., CreditCard, PayPal, BankTransfer).</summary>
    public PaymentMethod Method { get; private set; }

    /// <summary>
    /// External transaction ID from the payment gateway (e.g., Stripe charge ID, PayPal transaction ID).
    /// Null until the payment is processed. Set by MarkAsCompleted() upon successful processing.
    /// Used for reconciliation with the external payment provider.
    /// </summary>
    public string? TransactionId { get; private set; }

    /// <summary>
    /// Current status of the payment (Pending, Completed, Failed, or Refunded).
    /// Transitions are managed by domain methods (MarkAsCompleted, MarkAsFailed, MarkAsRefunded).
    /// </summary>
    public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;

    /// <summary>Timestamp of when the payment was successfully completed. Null until payment succeeds.</summary>
    public DateTime? PaidAt { get; private set; }

    /// <summary>Reason for payment failure (e.g., "Card declined", "Insufficient funds"). Null unless Status is Failed.</summary>
    public string? FailureReason { get; private set; }

    /// <summary>Private parameterless constructor required by EF Core for database materialization.</summary>
    private Payment() { }

    /// <summary>
    /// Factory method to create a new Payment in Pending status.
    /// Validates that the payment amount is positive.
    /// </summary>
    /// <param name="orderId">The ID of the order this payment is for.</param>
    /// <param name="amount">The payment amount (must be positive).</param>
    /// <param name="method">The payment method to use.</param>
    /// <param name="transactionId">Optional pre-assigned transaction ID from the payment gateway.</param>
    /// <returns>A new Payment in Pending status.</returns>
    /// <exception cref="ArgumentException">Thrown if amount is <= 0.</exception>
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

    /// <summary>
    /// Marks the payment as successfully completed.
    /// Sets the TransactionId (from the payment gateway), records the PaidAt timestamp,
    /// and transitions Status to Completed.
    /// </summary>
    /// <param name="transactionId">The transaction ID from the payment gateway (required).</param>
    /// <exception cref="ArgumentException">Thrown if transactionId is null or whitespace.</exception>
    public void MarkAsCompleted(string transactionId)
    {
        if (string.IsNullOrWhiteSpace(transactionId))
            throw new ArgumentException("Transaction ID is required.");

        Status = PaymentStatus.Completed;
        TransactionId = transactionId;
        PaidAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the payment as failed and records the reason for failure.
    /// This is called when the payment gateway reports a declined transaction or processing error.
    /// </summary>
    /// <param name="reason">A description of why the payment failed (e.g., "Card declined").</param>
    public void MarkAsFailed(string reason)
    {
        Status = PaymentStatus.Failed;
        FailureReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks a previously completed payment as refunded.
    /// This is called when a refund is processed (e.g., order cancellation, return).
    /// </summary>
    public void MarkAsRefunded()
    {
        Status = PaymentStatus.Refunded;
        UpdatedAt = DateTime.UtcNow;
    }
}

using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

/// <summary>
/// Specialized repository interface for Payment entities.
/// Extends the generic IRepository with Payment-specific query methods.
/// 
/// Provides queries for payment processing, reconciliation, and financial reporting:
/// looking up payments by order, by external transaction ID, and by payment status.
/// </summary>
public interface IPaymentRepository : IRepository<Payment>
{
    /// <summary>
    /// Retrieves the payment associated with a specific order.
    /// Used to check payment status during order processing and to display payment details on order summaries.
    /// </summary>
    /// <param name="orderId">The order's unique identifier.</param>
    /// <param name="cancellationToken">Token to cancel the async operation.</param>
    /// <returns>The payment for the order, or null if no payment exists.</returns>
    Task<Payment?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds a payment by its external transaction ID (from the payment gateway, e.g., Stripe charge ID).
    /// Used for webhook processing and payment reconciliation with external payment providers.
    /// </summary>
    /// <param name="transactionId">The external payment gateway transaction ID.</param>
    /// <param name="cancellationToken">Token to cancel the async operation.</param>
    /// <returns>The matching Payment, or null if not found.</returns>
    Task<Payment?> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all payments with a specific status (e.g., all Pending payments, all Failed payments).
    /// Used for financial dashboards, batch processing (e.g., retrying failed payments),
    /// and reporting on payment success/failure rates.
    /// </summary>
    /// <param name="status">The payment status to filter by.</param>
    /// <param name="cancellationToken">Token to cancel the async operation.</param>
    /// <returns>A read-only list of payments matching the specified status.</returns>
    Task<IReadOnlyList<Payment>> GetByStatusAsync(Enums.PaymentStatus status, CancellationToken cancellationToken = default);
}

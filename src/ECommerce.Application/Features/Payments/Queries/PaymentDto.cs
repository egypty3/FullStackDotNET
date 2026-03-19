namespace ECommerce.Application.Features.Payments.Queries;

public record PaymentDto
{
    public Guid Id { get; init; }
    public Guid OrderId { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "USD";
    public string Method { get; init; } = string.Empty;
    public string? TransactionId { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime? PaidAt { get; init; }
    public string? FailureReason { get; init; }
    public DateTime CreatedAt { get; init; }
}

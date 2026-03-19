namespace ECommerce.Application.Features.Orders.Queries;

public record OrderDto
{
    public Guid Id { get; init; }
    public string CustomerId { get; init; } = string.Empty;
    public string CustomerEmail { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public string Currency { get; init; } = "USD";
    public string PaymentStatus { get; init; } = string.Empty;
    public string? PaymentTransactionId { get; init; }
    public string ShippingStreet { get; init; } = string.Empty;
    public string ShippingCity { get; init; } = string.Empty;
    public string ShippingState { get; init; } = string.Empty;
    public string ShippingZipCode { get; init; } = string.Empty;
    public string ShippingCountry { get; init; } = string.Empty;
    public List<OrderItemDto> Items { get; init; } = new();
    public DateTime CreatedAt { get; init; }
}

public record OrderItemDto
{
    public Guid Id { get; init; }
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public decimal UnitPrice { get; init; }
    public int Quantity { get; init; }
    public decimal Subtotal { get; init; }
}

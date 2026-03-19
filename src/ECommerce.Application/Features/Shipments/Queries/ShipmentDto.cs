namespace ECommerce.Application.Features.Shipments.Queries;

public record ShipmentDto
{
    public Guid Id { get; init; }
    public Guid OrderId { get; init; }
    public Guid ShipperId { get; init; }
    public string ShipperName { get; init; } = string.Empty;
    public string TrackingNumber { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime? ShippedDate { get; init; }
    public DateTime? EstimatedDeliveryDate { get; init; }
    public DateTime? DeliveredDate { get; init; }
    public decimal ShippingCost { get; init; }
    public string? Notes { get; init; }
    public DateTime CreatedAt { get; init; }
}

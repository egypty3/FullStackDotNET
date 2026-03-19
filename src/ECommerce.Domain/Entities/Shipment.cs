using ECommerce.Domain.Common;
using ECommerce.Domain.Enums;

namespace ECommerce.Domain.Entities;

public class Shipment : BaseEntity
{
    public Guid OrderId { get; private set; }
    public Order Order { get; private set; } = null!;
    public Guid ShipperId { get; private set; }
    public Shipper Shipper { get; private set; } = null!;
    public string TrackingNumber { get; private set; } = string.Empty;
    public ShipmentStatus Status { get; private set; } = ShipmentStatus.Pending;
    public DateTime? ShippedDate { get; private set; }
    public DateTime? EstimatedDeliveryDate { get; private set; }
    public DateTime? DeliveredDate { get; private set; }
    public decimal ShippingCost { get; private set; }
    public string? Notes { get; private set; }

    private Shipment() { }

    public static Shipment Create(Guid orderId, Guid shipperId, string trackingNumber,
        decimal shippingCost, DateTime? estimatedDeliveryDate = null, string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(trackingNumber))
            throw new ArgumentException("Tracking number is required.");
        if (shippingCost < 0)
            throw new ArgumentException("Shipping cost cannot be negative.");

        return new Shipment
        {
            OrderId = orderId,
            ShipperId = shipperId,
            TrackingNumber = trackingNumber,
            ShippingCost = shippingCost,
            EstimatedDeliveryDate = estimatedDeliveryDate,
            Notes = notes
        };
    }

    public void MarkAsShipped()
    {
        Status = ShipmentStatus.Shipped;
        ShippedDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkInTransit()
    {
        Status = ShipmentStatus.InTransit;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkOutForDelivery()
    {
        Status = ShipmentStatus.OutForDelivery;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsDelivered()
    {
        Status = ShipmentStatus.Delivered;
        DeliveredDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsReturned()
    {
        Status = ShipmentStatus.Returned;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateTracking(string trackingNumber, string? notes)
    {
        TrackingNumber = trackingNumber;
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }
}

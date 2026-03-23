using ECommerce.Domain.Common;
using ECommerce.Domain.Enums;

namespace ECommerce.Domain.Entities;

/// <summary>
/// Represents the physical shipment/delivery of an order to the customer.
/// Tracks the package from dispatch through final delivery (or return).
/// 
/// Shipment Lifecycle (Status transitions):
///   Pending -> Shipped -> InTransit -> OutForDelivery -> Delivered
///                                                       |
///                                                       +-> Returned
/// 
/// Key design aspects:
/// - Each Shipment is linked to one Order and one Shipper (the carrier company).
/// - TrackingNumber provides the carrier's tracking code for package tracking.
/// - Captures key timestamps: ShippedDate, EstimatedDeliveryDate, and actual DeliveredDate.
/// - ShippingCost records the freight/delivery charge for this shipment.
/// - Notes field allows adding delivery instructions or special handling notes.
/// 
/// Inherits Id, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy from BaseEntity.
/// </summary>
public class Shipment : BaseEntity
{
    /// <summary>Foreign key to the Order being shipped.</summary>
    public Guid OrderId { get; private set; }

    /// <summary>Navigation property to the associated Order entity. Populated by EF Core.</summary>
    public Order Order { get; private set; } = null!;

    /// <summary>Foreign key to the Shipper (carrier company) handling this shipment.</summary>
    public Guid ShipperId { get; private set; }

    /// <summary>Navigation property to the Shipper entity (carrier company). Populated by EF Core.</summary>
    public Shipper Shipper { get; private set; } = null!;

    /// <summary>
    /// The carrier's tracking number for this shipment.
    /// Used by customers to track their package on the carrier's website.
    /// Required at creation time.
    /// </summary>
    public string TrackingNumber { get; private set; } = string.Empty;

    /// <summary>
    /// Current status of the shipment in the delivery lifecycle.
    /// Transitions are managed by the Mark* methods (MarkAsShipped, MarkInTransit, etc.).
    /// </summary>
    public ShipmentStatus Status { get; private set; } = ShipmentStatus.Pending;

    /// <summary>Timestamp of when the package was actually dispatched. Set by MarkAsShipped().</summary>
    public DateTime? ShippedDate { get; private set; }

    /// <summary>The expected/projected delivery date provided by the carrier. Set at creation time.</summary>
    public DateTime? EstimatedDeliveryDate { get; private set; }

    /// <summary>Timestamp of when the package was actually delivered. Set by MarkAsDelivered().</summary>
    public DateTime? DeliveredDate { get; private set; }

    /// <summary>The cost of shipping this package. Uses decimal for financial precision. Cannot be negative.</summary>
    public decimal ShippingCost { get; private set; }

    /// <summary>Optional notes about the shipment (e.g., "Leave at front door", "Fragile - handle with care").</summary>
    public string? Notes { get; private set; }

    /// <summary>Private parameterless constructor required by EF Core for database materialization.</summary>
    private Shipment() { }

    /// <summary>
    /// Factory method to create a new Shipment in Pending status.
    /// Validates that the tracking number is provided and shipping cost is non-negative.
    /// </summary>
    /// <param name="orderId">The ID of the order being shipped.</param>
    /// <param name="shipperId">The ID of the carrier company handling the shipment.</param>
    /// <param name="trackingNumber">The carrier's tracking number (required).</param>
    /// <param name="shippingCost">The cost of shipping (must be >= 0).</param>
    /// <param name="estimatedDeliveryDate">Optional estimated delivery date.</param>
    /// <param name="notes">Optional shipment notes or delivery instructions.</param>
    /// <returns>A new Shipment in Pending status.</returns>
    /// <exception cref="ArgumentException">Thrown if trackingNumber is empty or shippingCost is negative.</exception>
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

    /// <summary>
    /// Transitions the shipment to Shipped status and records the ship date.
    /// Called when the package is handed off to the carrier.
    /// </summary>
    public void MarkAsShipped()
    {
        Status = ShipmentStatus.Shipped;
        ShippedDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Transitions the shipment to InTransit status.
    /// Called when the carrier confirms the package is moving between distribution centers.
    /// </summary>
    public void MarkInTransit()
    {
        Status = ShipmentStatus.InTransit;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Transitions the shipment to OutForDelivery status.
    /// Called when the package is on the final delivery vehicle heading to the customer.
    /// </summary>
    public void MarkOutForDelivery()
    {
        Status = ShipmentStatus.OutForDelivery;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Transitions the shipment to Delivered status and records the delivery date.
    /// Called when delivery confirmation is received from the carrier.
    /// </summary>
    public void MarkAsDelivered()
    {
        Status = ShipmentStatus.Delivered;
        DeliveredDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Transitions the shipment to Returned status.
    /// Called when the package is returned (delivery failed, customer refused, or return initiated).
    /// </summary>
    public void MarkAsReturned()
    {
        Status = ShipmentStatus.Returned;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the tracking number and notes for this shipment.
    /// Used when the carrier provides an updated tracking number or when delivery instructions change.
    /// </summary>
    /// <param name="trackingNumber">The new tracking number.</param>
    /// <param name="notes">Updated notes (null to clear).</param>
    public void UpdateTracking(string trackingNumber, string? notes)
    {
        TrackingNumber = trackingNumber;
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }
}

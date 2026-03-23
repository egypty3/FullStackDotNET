using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

/// <summary>
/// Specialized repository interface for Shipment entities.
/// Extends the generic IRepository with Shipment-specific query methods.
/// 
/// Provides queries for tracking shipments by order, carrier tracking number,
/// and by the shipper (carrier) handling the delivery.
/// </summary>
public interface IShipmentRepository : IRepository<Shipment>
{
    /// <summary>
    /// Retrieves the shipment associated with a specific order.
    /// Used to display shipping status and tracking information on the order detail page.
    /// </summary>
    /// <param name="orderId">The order's unique identifier.</param>
    /// <param name="cancellationToken">Token to cancel the async operation.</param>
    /// <returns>The shipment for the order, or null if no shipment has been created yet.</returns>
    Task<Shipment?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds a shipment by the carrier's tracking number.
    /// Used for package tracking lookups and webhook processing from carrier APIs.
    /// </summary>
    /// <param name="trackingNumber">The carrier tracking number to search for.</param>
    /// <param name="cancellationToken">Token to cancel the async operation.</param>
    /// <returns>The matching Shipment, or null if not found.</returns>
    Task<Shipment?> GetByTrackingNumberAsync(string trackingNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all shipments handled by a specific shipper (carrier company).
    /// Used for carrier performance reporting and managing the relationship with shipping partners.
    /// </summary>
    /// <param name="shipperId">The shipper's unique identifier.</param>
    /// <param name="cancellationToken">Token to cancel the async operation.</param>
    /// <returns>A read-only list of shipments handled by the specified shipper.</returns>
    Task<IReadOnlyList<Shipment>> GetByShipperIdAsync(Guid shipperId, CancellationToken cancellationToken = default);
}

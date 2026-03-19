using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

public interface IShipmentRepository : IRepository<Shipment>
{
    Task<Shipment?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Shipment?> GetByTrackingNumberAsync(string trackingNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Shipment>> GetByShipperIdAsync(Guid shipperId, CancellationToken cancellationToken = default);
}

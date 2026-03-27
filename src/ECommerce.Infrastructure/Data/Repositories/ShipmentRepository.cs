using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Data.Repositories;

/// <summary>
/// Repository for <see cref="Shipment"/> entities. Overrides the base <c>GetByIdAsync</c> to
/// eager-load related <see cref="Order"/> and <see cref="Shipper"/> navigation properties,
/// and provides look-ups by order ID, tracking number, and shipper.
/// </summary>
public class ShipmentRepository : Repository<Shipment>, IShipmentRepository
{
    /// <inheritdoc />
    public ShipmentRepository(ApplicationDbContext context) : base(context) { }

    /// <summary>
    /// Retrieves a shipment by ID with its related <see cref="Order"/> and <see cref="Shipper"/> included.
    /// </summary>
    public override async Task<Shipment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbSet.Include(s => s.Order).Include(s => s.Shipper)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    /// <summary>
    /// Finds the shipment associated with the given <paramref name="orderId"/>,
    /// including the shipper details.
    /// </summary>
    public async Task<Shipment?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
        => await _dbSet.Include(s => s.Shipper)
            .FirstOrDefaultAsync(s => s.OrderId == orderId, cancellationToken);

    /// <summary>
    /// Finds a shipment by its unique carrier <paramref name="trackingNumber"/>,
    /// including both the order and shipper details.
    /// </summary>
    public async Task<Shipment?> GetByTrackingNumberAsync(string trackingNumber, CancellationToken cancellationToken = default)
        => await _dbSet.Include(s => s.Order).Include(s => s.Shipper)
            .FirstOrDefaultAsync(s => s.TrackingNumber == trackingNumber, cancellationToken);

    /// <summary>
    /// Returns all shipments handled by the specified <paramref name="shipperId"/>,
    /// ordered by newest first, with the related order included.
    /// </summary>
    public async Task<IReadOnlyList<Shipment>> GetByShipperIdAsync(Guid shipperId, CancellationToken cancellationToken = default)
        => await _dbSet.Include(s => s.Order)
            .Where(s => s.ShipperId == shipperId).OrderByDescending(s => s.CreatedAt).ToListAsync(cancellationToken);
}

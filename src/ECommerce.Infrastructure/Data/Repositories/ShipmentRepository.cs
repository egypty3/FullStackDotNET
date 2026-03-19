using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Data.Repositories;

public class ShipmentRepository : Repository<Shipment>, IShipmentRepository
{
    public ShipmentRepository(ApplicationDbContext context) : base(context) { }

    public override async Task<Shipment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbSet.Include(s => s.Order).Include(s => s.Shipper)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<Shipment?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
        => await _dbSet.Include(s => s.Shipper)
            .FirstOrDefaultAsync(s => s.OrderId == orderId, cancellationToken);

    public async Task<Shipment?> GetByTrackingNumberAsync(string trackingNumber, CancellationToken cancellationToken = default)
        => await _dbSet.Include(s => s.Order).Include(s => s.Shipper)
            .FirstOrDefaultAsync(s => s.TrackingNumber == trackingNumber, cancellationToken);

    public async Task<IReadOnlyList<Shipment>> GetByShipperIdAsync(Guid shipperId, CancellationToken cancellationToken = default)
        => await _dbSet.Include(s => s.Order)
            .Where(s => s.ShipperId == shipperId).OrderByDescending(s => s.CreatedAt).ToListAsync(cancellationToken);
}

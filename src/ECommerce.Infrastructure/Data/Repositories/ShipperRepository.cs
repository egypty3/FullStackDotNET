using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Data.Repositories;

public class ShipperRepository : Repository<Shipper>, IShipperRepository
{
    public ShipperRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Shipper?> GetByNameAsync(string companyName, CancellationToken cancellationToken = default)
        => await _dbSet.FirstOrDefaultAsync(s => s.CompanyName == companyName, cancellationToken);

    public async Task<IReadOnlyList<Shipper>> GetActiveShippersAsync(CancellationToken cancellationToken = default)
        => await _dbSet.Where(s => s.IsActive).OrderBy(s => s.CompanyName).ToListAsync(cancellationToken);
}

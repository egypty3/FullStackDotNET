using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Data.Repositories;

/// <summary>
/// Repository for <see cref="Shipper"/> entities. Supports look-up by company name
/// and retrieval of active shipping carriers sorted alphabetically.
/// </summary>
public class ShipperRepository : Repository<Shipper>, IShipperRepository
{
    /// <inheritdoc />
    public ShipperRepository(ApplicationDbContext context) : base(context) { }

    /// <summary>
    /// Finds a shipper by its exact <paramref name="companyName"/>. Returns <c>null</c> if not found.
    /// </summary>
    public async Task<Shipper?> GetByNameAsync(string companyName, CancellationToken cancellationToken = default)
        => await _dbSet.FirstOrDefaultAsync(s => s.CompanyName == companyName, cancellationToken);

    /// <summary>
    /// Returns all active shippers ordered alphabetically by company name.
    /// </summary>
    public async Task<IReadOnlyList<Shipper>> GetActiveShippersAsync(CancellationToken cancellationToken = default)
        => await _dbSet.Where(s => s.IsActive).OrderBy(s => s.CompanyName).ToListAsync(cancellationToken);
}

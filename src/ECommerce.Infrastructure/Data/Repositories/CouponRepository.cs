using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Data.Repositories;

public class CouponRepository : Repository<Coupon>, ICouponRepository
{
    public CouponRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Coupon?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        => await _dbSet.FirstOrDefaultAsync(c => c.Code == code.ToUpperInvariant(), cancellationToken);

    public async Task<IReadOnlyList<Coupon>> GetActiveCouponsAsync(CancellationToken cancellationToken = default)
        => await _dbSet.Where(c => c.IsActive && c.ExpiresAt > DateTime.UtcNow && c.CurrentUses < c.MaxUses)
            .OrderBy(c => c.ExpiresAt).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Coupon>> GetExpiredCouponsAsync(CancellationToken cancellationToken = default)
        => await _dbSet.Where(c => c.ExpiresAt <= DateTime.UtcNow || c.CurrentUses >= c.MaxUses)
            .OrderByDescending(c => c.ExpiresAt).ToListAsync(cancellationToken);
}

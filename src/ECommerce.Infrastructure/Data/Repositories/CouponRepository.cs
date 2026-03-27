using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Data.Repositories;

/// <summary>
/// Repository for <see cref="Coupon"/> entities. Supports case-insensitive look-up by
/// coupon code and retrieval of active or expired coupons.
/// </summary>
public class CouponRepository : Repository<Coupon>, ICouponRepository
{
    /// <inheritdoc />
    public CouponRepository(ApplicationDbContext context) : base(context) { }

    /// <summary>
    /// Finds a coupon by its case-insensitive <paramref name="code"/> (converted to upper-case).
    /// Returns <c>null</c> if no matching coupon exists.
    /// </summary>
    public async Task<Coupon?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        => await _dbSet.FirstOrDefaultAsync(c => c.Code == code.ToUpperInvariant(), cancellationToken);

    /// <summary>
    /// Returns all coupons that are currently active (enabled, not expired, and not fully redeemed),
    /// ordered by the soonest expiration date first.
    /// </summary>
    public async Task<IReadOnlyList<Coupon>> GetActiveCouponsAsync(CancellationToken cancellationToken = default)
        => await _dbSet.Where(c => c.IsActive && c.ExpiresAt > DateTime.UtcNow && c.CurrentUses < c.MaxUses)
            .OrderBy(c => c.ExpiresAt).ToListAsync(cancellationToken);

    /// <summary>
    /// Returns all coupons that have expired or been fully redeemed,
    /// ordered by most recently expired first.
    /// </summary>
    public async Task<IReadOnlyList<Coupon>> GetExpiredCouponsAsync(CancellationToken cancellationToken = default)
        => await _dbSet.Where(c => c.ExpiresAt <= DateTime.UtcNow || c.CurrentUses >= c.MaxUses)
            .OrderByDescending(c => c.ExpiresAt).ToListAsync(cancellationToken);
}

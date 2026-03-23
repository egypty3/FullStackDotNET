using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

/// <summary>
/// Specialized repository interface for Coupon entities.
/// Extends the generic IRepository with Coupon-specific query methods.
/// 
/// Provides queries needed by the checkout flow (code lookup) and admin management
/// (listing active/expired coupons for monitoring and cleanup).
/// </summary>
public interface ICouponRepository : IRepository<Coupon>
{
    /// <summary>
    /// Finds a coupon by its unique code string. Returns null if no coupon with that code exists.
    /// This is the primary lookup method used during checkout when a customer enters a coupon code.
    /// </summary>
    /// <param name="code">The coupon code to search for (case handling depends on implementation).</param>
    /// <param name="cancellationToken">Token to cancel the async operation.</param>
    /// <returns>The matching Coupon, or null if not found.</returns>
    Task<Coupon?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all coupons where IsActive is true.
    /// Used by admin dashboards to show currently available promotions.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the async operation.</param>
    /// <returns>A read-only list of active coupons.</returns>
    Task<IReadOnlyList<Coupon>> GetActiveCouponsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all coupons that have passed their expiration date.
    /// Used by admin tools for cleanup, reporting, or batch deactivation of expired promotions.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the async operation.</param>
    /// <returns>A read-only list of expired coupons.</returns>
    Task<IReadOnlyList<Coupon>> GetExpiredCouponsAsync(CancellationToken cancellationToken = default);
}

using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Data.Repositories;

/// <summary>
/// Repository for <see cref="Review"/> entities. Overrides the base <c>GetByIdAsync</c> to
/// eager-load both <see cref="Product"/> and <see cref="Customer"/>, and provides queries
/// to filter reviews by product, customer, approval status, and pending moderation.
/// </summary>
public class ReviewRepository : Repository<Review>, IReviewRepository
{
    /// <inheritdoc />
    public ReviewRepository(ApplicationDbContext context) : base(context) { }

    /// <summary>
    /// Retrieves a review by ID with its related <see cref="Product"/> and <see cref="Customer"/> included.
    /// </summary>
    public override async Task<Review?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbSet.Include(r => r.Product).Include(r => r.Customer)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    /// <summary>
    /// Returns all reviews for a specific product (both approved and pending),
    /// ordered by newest first, with customer details included.
    /// </summary>
    public async Task<IReadOnlyList<Review>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
        => await _dbSet.Include(r => r.Customer)
            .Where(r => r.ProductId == productId).OrderByDescending(r => r.CreatedAt).ToListAsync(cancellationToken);

    /// <summary>
    /// Returns all reviews written by a specific customer, ordered by newest first,
    /// with product details included.
    /// </summary>
    public async Task<IReadOnlyList<Review>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
        => await _dbSet.Include(r => r.Product)
            .Where(r => r.CustomerId == customerId).OrderByDescending(r => r.CreatedAt).ToListAsync(cancellationToken);

    /// <summary>
    /// Returns only approved reviews for a specific product, suitable for public display.
    /// Ordered by newest first with customer details included.
    /// </summary>
    public async Task<IReadOnlyList<Review>> GetApprovedByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
        => await _dbSet.Include(r => r.Customer)
            .Where(r => r.ProductId == productId && r.IsApproved).OrderByDescending(r => r.CreatedAt).ToListAsync(cancellationToken);

    /// <summary>
    /// Returns all reviews awaiting moderator approval, ordered by oldest first
    /// (FIFO for the moderation queue), with product and customer details included.
    /// </summary>
    public async Task<IReadOnlyList<Review>> GetPendingReviewsAsync(CancellationToken cancellationToken = default)
        => await _dbSet.Include(r => r.Product).Include(r => r.Customer)
            .Where(r => !r.IsApproved).OrderBy(r => r.CreatedAt).ToListAsync(cancellationToken);
}

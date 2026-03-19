using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Data.Repositories;

public class ReviewRepository : Repository<Review>, IReviewRepository
{
    public ReviewRepository(ApplicationDbContext context) : base(context) { }

    public override async Task<Review?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbSet.Include(r => r.Product).Include(r => r.Customer)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Review>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
        => await _dbSet.Include(r => r.Customer)
            .Where(r => r.ProductId == productId).OrderByDescending(r => r.CreatedAt).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Review>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
        => await _dbSet.Include(r => r.Product)
            .Where(r => r.CustomerId == customerId).OrderByDescending(r => r.CreatedAt).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Review>> GetApprovedByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
        => await _dbSet.Include(r => r.Customer)
            .Where(r => r.ProductId == productId && r.IsApproved).OrderByDescending(r => r.CreatedAt).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Review>> GetPendingReviewsAsync(CancellationToken cancellationToken = default)
        => await _dbSet.Include(r => r.Product).Include(r => r.Customer)
            .Where(r => !r.IsApproved).OrderBy(r => r.CreatedAt).ToListAsync(cancellationToken);
}

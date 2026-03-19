using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

public interface IReviewRepository : IRepository<Review>
{
    Task<IReadOnlyList<Review>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Review>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Review>> GetApprovedByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Review>> GetPendingReviewsAsync(CancellationToken cancellationToken = default);
}

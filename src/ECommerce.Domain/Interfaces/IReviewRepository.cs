using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

/// <summary>
/// Specialized repository interface for Review entities.
/// Extends the generic IRepository with Review-specific query methods.
/// 
/// Provides queries for both the customer-facing product pages (approved reviews)
/// and the admin moderation workflow (pending reviews).
/// </summary>
public interface IReviewRepository : IRepository<Review>
{
    /// <summary>
    /// Retrieves all reviews (approved and unapproved) for a specific product.
    /// Used by admin views to see all reviews for a product, including those awaiting moderation.
    /// </summary>
    /// <param name="productId">The product's unique identifier.</param>
    /// <param name="cancellationToken">Token to cancel the async operation.</param>
    /// <returns>A read-only list of all reviews for the product.</returns>
    Task<IReadOnlyList<Review>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all reviews written by a specific customer.
    /// Used for the customer's "My Reviews" page and for admin investigation of a customer's review activity.
    /// </summary>
    /// <param name="customerId">The customer's unique identifier.</param>
    /// <param name="cancellationToken">Token to cancel the async operation.</param>
    /// <returns>A read-only list of all reviews by the customer.</returns>
    Task<IReadOnlyList<Review>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves only approved reviews for a specific product.
    /// Used on the public product detail page — only moderation-approved reviews are shown to customers.
    /// </summary>
    /// <param name="productId">The product's unique identifier.</param>
    /// <param name="cancellationToken">Token to cancel the async operation.</param>
    /// <returns>A read-only list of approved reviews for the product.</returns>
    Task<IReadOnlyList<Review>> GetApprovedByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all reviews that have not yet been approved (IsApproved == false).
    /// Used by the admin moderation queue to show reviews awaiting approval or rejection.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the async operation.</param>
    /// <returns>A read-only list of reviews pending moderation.</returns>
    Task<IReadOnlyList<Review>> GetPendingReviewsAsync(CancellationToken cancellationToken = default);
}

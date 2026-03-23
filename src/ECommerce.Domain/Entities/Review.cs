using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities;

/// <summary>
/// Represents a customer's review of a product, including a numeric rating and text feedback.
/// 
/// Key design aspects:
/// - Reviews require a rating (1-5 stars) and a title. Comments are optional.
/// - New reviews start as unapproved (IsApproved = false) and must go through a moderation workflow
///   (Approve/Reject methods) before being visible to other customers.
/// - Links to both a Product and a Customer via navigation properties, enabling queries like
///   "all reviews for product X" or "all reviews by customer Y".
/// 
/// The moderation pattern (approve/reject) prevents spam and inappropriate content from appearing
/// in the product catalog without manual or automated review.
/// 
/// Inherits Id, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy from BaseEntity.
/// </summary>
public class Review : BaseEntity
{
    /// <summary>Foreign key to the Product being reviewed.</summary>
    public Guid ProductId { get; private set; }

    /// <summary>
    /// Navigation property to the reviewed Product entity.
    /// Populated by EF Core when included in a query.
    /// </summary>
    public Product Product { get; private set; } = null!;

    /// <summary>Foreign key to the Customer who wrote the review.</summary>
    public Guid CustomerId { get; private set; }

    /// <summary>
    /// Navigation property to the Customer who authored the review.
    /// Populated by EF Core when included in a query.
    /// </summary>
    public Customer Customer { get; private set; } = null!;

    /// <summary>
    /// Numeric rating from 1 to 5 stars. Validated during creation and updates.
    /// Used for calculating average product ratings and sorting by rating.
    /// </summary>
    public int Rating { get; private set; }

    /// <summary>The headline/summary of the review (required). Displayed prominently in review listings.</summary>
    public string Title { get; private set; } = string.Empty;

    /// <summary>The detailed review text/body. Can be empty if the customer only wants to leave a rating and title.</summary>
    public string Comment { get; private set; } = string.Empty;

    /// <summary>
    /// Moderation flag: indicates whether the review has been approved by a moderator/admin.
    /// New reviews start as unapproved (false). Only approved reviews are shown publicly.
    /// Managed by the Approve() and Reject() methods.
    /// </summary>
    public bool IsApproved { get; private set; }

    /// <summary>Private parameterless constructor required by EF Core for database materialization.</summary>
    private Review() { }

    /// <summary>
    /// Factory method to create a new Review in unapproved state.
    /// Validates that the rating is within the 1-5 range and the title is not empty.
    /// </summary>
    /// <param name="productId">The ID of the product being reviewed.</param>
    /// <param name="customerId">The ID of the customer writing the review.</param>
    /// <param name="rating">A rating from 1 (worst) to 5 (best).</param>
    /// <param name="title">The review title/headline (required).</param>
    /// <param name="comment">The detailed review text (can be empty).</param>
    /// <returns>A new unapproved Review instance.</returns>
    /// <exception cref="ArgumentException">Thrown if rating is not 1-5 or title is empty.</exception>
    public static Review Create(Guid productId, Guid customerId, int rating, string title, string comment)
    {
        if (rating < 1 || rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5.");
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Review title is required.");

        return new Review
        {
            ProductId = productId,
            CustomerId = customerId,
            Rating = rating,
            Title = title,
            Comment = comment,
            IsApproved = false // New reviews require moderation before becoming visible
        };
    }

    /// <summary>
    /// Updates the review's rating, title, and comment.
    /// Validates the rating range. Typically called when a customer edits their own review.
    /// Note: An updated review may need re-approval depending on business policy.
    /// </summary>
    /// <param name="rating">Updated rating (1-5).</param>
    /// <param name="title">Updated review title.</param>
    /// <param name="comment">Updated review text.</param>
    /// <exception cref="ArgumentException">Thrown if rating is not in the 1-5 range.</exception>
    public void Update(int rating, string title, string comment)
    {
        if (rating < 1 || rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5.");

        Rating = rating;
        Title = title;
        Comment = comment;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Approves the review, making it visible to other customers on the product page.
    /// Typically called by an admin/moderator after reviewing the content.
    /// </summary>
    public void Approve()
    {
        IsApproved = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Rejects the review, keeping it hidden from public view.
    /// Typically called by an admin/moderator if the content is inappropriate or spam.
    /// </summary>
    public void Reject()
    {
        IsApproved = false;
        UpdatedAt = DateTime.UtcNow;
    }
}

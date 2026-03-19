using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities;

public class Review : BaseEntity
{
    public Guid ProductId { get; private set; }
    public Product Product { get; private set; } = null!;
    public Guid CustomerId { get; private set; }
    public Customer Customer { get; private set; } = null!;
    public int Rating { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Comment { get; private set; } = string.Empty;
    public bool IsApproved { get; private set; }

    private Review() { }

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
            IsApproved = false
        };
    }

    public void Update(int rating, string title, string comment)
    {
        if (rating < 1 || rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5.");

        Rating = rating;
        Title = title;
        Comment = comment;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Approve()
    {
        IsApproved = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject()
    {
        IsApproved = false;
        UpdatedAt = DateTime.UtcNow;
    }
}

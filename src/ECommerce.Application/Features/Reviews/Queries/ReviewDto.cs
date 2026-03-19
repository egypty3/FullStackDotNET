namespace ECommerce.Application.Features.Reviews.Queries;

public record ReviewDto
{
    public Guid Id { get; init; }
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public Guid CustomerId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public int Rating { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Comment { get; init; } = string.Empty;
    public bool IsApproved { get; init; }
    public DateTime CreatedAt { get; init; }
}

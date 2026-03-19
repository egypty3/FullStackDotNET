namespace ECommerce.Application.Features.Products.Queries;

public record ProductDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string Currency { get; init; } = "USD";
    public int StockQuantity { get; init; }
    public string SKU { get; init; } = string.Empty;
    public Guid CategoryId { get; init; }
    public string CategoryName { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public string? ImageUrl { get; init; }
    public DateTime CreatedAt { get; init; }
}

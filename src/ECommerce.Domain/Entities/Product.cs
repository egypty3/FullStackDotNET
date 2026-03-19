using ECommerce.Domain.Common;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public Money Price { get; private set; } = new(0);
    public int StockQuantity { get; private set; }
    public string SKU { get; private set; } = string.Empty;
    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; } = null!;
    public bool IsActive { get; private set; } = true;
    public string? ImageUrl { get; private set; }

    private Product() { } // EF Core

    public static Product Create(
        string name, string description, decimal price,
        int stockQuantity, string sku, Guid categoryId, string? imageUrl = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name is required.");
        if (price < 0)
            throw new ArgumentException("Price cannot be negative.");
        if (stockQuantity < 0)
            throw new ArgumentException("Stock quantity cannot be negative.");

        return new Product
        {
            Name = name,
            Description = description,
            Price = new Money(price),
            StockQuantity = stockQuantity,
            SKU = sku,
            CategoryId = categoryId,
            ImageUrl = imageUrl
        };
    }

    public void Update(string name, string description, decimal price,
        int stockQuantity, Guid categoryId, string? imageUrl)
    {
        Name = name;
        Description = description;
        Price = new Money(price);
        StockQuantity = stockQuantity;
        CategoryId = categoryId;
        ImageUrl = imageUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ReduceStock(int quantity)
    {
        if (quantity > StockQuantity)
            throw new InvalidOperationException($"Insufficient stock. Available: {StockQuantity}");
        StockQuantity -= quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive.");
        StockQuantity += quantity;
        UpdatedAt = DateTime.UtcNow;
    }
}

using ECommerce.Domain.Common;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Domain.Entities;

/// <summary>
/// Represents a product available for sale in the ECommerce catalog.
/// 
/// Key design aspects:
/// - Price is stored as a Money value object (not raw decimal) to maintain currency information.
/// - SKU (Stock Keeping Unit) is a unique identifier used for inventory management and product lookups.
/// - Has a many-to-one relationship with Category (each product belongs to one category).
/// - Manages its own inventory through ReduceStock() and AddStock() methods with business rule validation.
/// - Supports soft-delete via IsActive flag (products can be hidden from the catalog without deletion).
/// 
/// Business invariants enforced:
/// - Price cannot be negative.
/// - Stock quantity cannot be negative.
/// - Stock reduction cannot exceed available quantity (prevents overselling).
/// - Stock addition requires a positive quantity.
/// 
/// Inherits Id, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy from BaseEntity.
/// </summary>
public class Product : BaseEntity
{
    /// <summary>The product's display name (required). Shown in the catalog, search results, and order details.</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>Detailed product description for the product detail page.</summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// The product's selling price, stored as a Money value object (includes currency).
    /// Wrapped in Money to enforce non-negative validation and currency tracking.
    /// </summary>
    public Money Price { get; private set; } = new(0);

    /// <summary>
    /// The number of units currently available in inventory.
    /// Managed through ReduceStock() (when orders are placed) and AddStock() (when restocking).
    /// </summary>
    public int StockQuantity { get; private set; }

    /// <summary>
    /// Stock Keeping Unit — a unique identifier for inventory management.
    /// Used in warehouse operations, barcode scanning, and inventory tracking systems.
    /// </summary>
    public string SKU { get; private set; } = string.Empty;

    /// <summary>Foreign key to the Category this product belongs to.</summary>
    public Guid CategoryId { get; private set; }

    /// <summary>
    /// Navigation property to the associated Category entity.
    /// Populated by EF Core when included in a query. The null-forgiving operator (null!)
    /// is used because EF Core guarantees this is populated when the relationship is loaded.
    /// </summary>
    public Category Category { get; private set; } = null!;

    /// <summary>Soft-delete flag. Inactive products are hidden from the catalog but preserved in historical orders.</summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>Optional URL to the product's image. Nullable for products without images.</summary>
    public string? ImageUrl { get; private set; }

    /// <summary>Private parameterless constructor required by EF Core for database materialization.</summary>
    private Product() { } // EF Core

    /// <summary>
    /// Factory method to create a new Product with validation.
    /// Validates that the name is not empty, price is non-negative, and stock is non-negative.
    /// Price is wrapped in a Money value object during creation.
    /// </summary>
    /// <param name="name">Product name (required).</param>
    /// <param name="description">Product description.</param>
    /// <param name="price">Product price (must be >= 0).</param>
    /// <param name="stockQuantity">Initial stock quantity (must be >= 0).</param>
    /// <param name="sku">Unique Stock Keeping Unit code.</param>
    /// <param name="categoryId">The ID of the category this product belongs to.</param>
    /// <param name="imageUrl">Optional product image URL.</param>
    /// <returns>A new active Product instance.</returns>
    /// <exception cref="ArgumentException">Thrown if name is empty, price < 0, or stockQuantity < 0.</exception>
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
            Price = new Money(price), // Wrap raw decimal in Money value object
            StockQuantity = stockQuantity,
            SKU = sku,
            CategoryId = categoryId,
            ImageUrl = imageUrl
        };
    }

    /// <summary>
    /// Updates the product's catalog information.
    /// Replaces all editable fields. The SKU is not updatable to maintain inventory tracking consistency.
    /// </summary>
    /// <param name="name">Updated product name.</param>
    /// <param name="description">Updated description.</param>
    /// <param name="price">Updated price (will be wrapped in a new Money value object).</param>
    /// <param name="stockQuantity">Updated stock quantity.</param>
    /// <param name="categoryId">Updated category assignment.</param>
    /// <param name="imageUrl">Updated image URL (null to remove).</param>
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

    /// <summary>
    /// Soft-deletes the product by setting IsActive to false.
    /// The product will be hidden from the catalog but remains in order history.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Reduces the stock quantity by the specified amount (e.g., when an order is placed).
    /// Enforces the business invariant that stock cannot go negative (prevents overselling).
    /// </summary>
    /// <param name="quantity">The number of units to subtract from stock.</param>
    /// <exception cref="InvalidOperationException">Thrown if the quantity exceeds available stock.</exception>
    public void ReduceStock(int quantity)
    {
        if (quantity > StockQuantity)
            throw new InvalidOperationException($"Insufficient stock. Available: {StockQuantity}");
        StockQuantity -= quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Increases the stock quantity by the specified amount (e.g., when receiving a new shipment from a supplier).
    /// Validates that the quantity being added is positive.
    /// </summary>
    /// <param name="quantity">The number of units to add to stock (must be > 0).</param>
    /// <exception cref="ArgumentException">Thrown if quantity is <= 0.</exception>
    public void AddStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive.");
        StockQuantity += quantity;
        UpdatedAt = DateTime.UtcNow;
    }
}

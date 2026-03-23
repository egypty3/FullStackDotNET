using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

/// <summary>
/// Specialized repository interface for Product entities.
/// Extends the generic IRepository with Product-specific query methods.
/// 
/// Provides queries essential for the product catalog: listing active products,
/// looking up by SKU (for inventory management), and filtering by category.
/// </summary>
public interface IProductRepository : IRepository<Product>
{
    /// <summary>
    /// Retrieves all products where IsActive is true.
    /// Used for the public-facing product catalog — only active products are shown to customers.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the async operation.</param>
    /// <returns>A read-only list of active products.</returns>
    Task<IReadOnlyList<Product>> GetActiveProductsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds a product by its unique SKU (Stock Keeping Unit) code.
    /// Used for inventory management, barcode scanning, and import/export operations.
    /// Returns null if no product with that SKU exists.
    /// </summary>
    /// <param name="sku">The SKU code to search for.</param>
    /// <param name="cancellationToken">Token to cancel the async operation.</param>
    /// <returns>The matching Product, or null if not found.</returns>
    Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all products belonging to a specific category.
    /// Used for the category browsing page where customers view all products within a chosen category.
    /// </summary>
    /// <param name="categoryId">The category's unique identifier to filter by.</param>
    /// <param name="cancellationToken">Token to cancel the async operation.</param>
    /// <returns>A read-only list of products in the specified category.</returns>
    Task<IReadOnlyList<Product>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);
}

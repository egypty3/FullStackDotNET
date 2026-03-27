using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Data.Repositories;

/// <summary>
/// Repository for <see cref="Product"/> entities. Overrides the base <c>GetByIdAsync</c> to
/// eager-load the related <see cref="Category"/> and adds domain-specific queries such as
/// filtering by SKU, category, or active status.
/// </summary>
public class ProductRepository : Repository<Product>, IProductRepository
{
    /// <inheritdoc />
    public ProductRepository(ApplicationDbContext context) : base(context) { }

    /// <summary>
    /// Retrieves a product by ID with its related <see cref="Category"/> eagerly loaded.
    /// </summary>
    public override async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbSet.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    /// <summary>
    /// Returns all active products ordered by newest first, with their categories included.
    /// </summary>
    public async Task<IReadOnlyList<Product>> GetActiveProductsAsync(CancellationToken cancellationToken = default)
        => await _dbSet.Where(p => p.IsActive).Include(p => p.Category).OrderByDescending(p => p.CreatedAt).ToListAsync(cancellationToken);

    /// <summary>
    /// Looks up a single product by its unique Stock Keeping Unit (SKU) code.
    /// </summary>
    public async Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
        => await _dbSet.FirstOrDefaultAsync(p => p.SKU == sku, cancellationToken);

    /// <summary>
    /// Returns all active products belonging to the specified <paramref name="categoryId"/>.
    /// </summary>
    public async Task<IReadOnlyList<Product>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
        => await _dbSet.Where(p => p.CategoryId == categoryId && p.IsActive).Include(p => p.Category).ToListAsync(cancellationToken);
}

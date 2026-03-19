using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Data.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context) { }

    public override async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbSet.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Product>> GetActiveProductsAsync(CancellationToken cancellationToken = default)
        => await _dbSet.Where(p => p.IsActive).Include(p => p.Category).OrderByDescending(p => p.CreatedAt).ToListAsync(cancellationToken);

    public async Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
        => await _dbSet.FirstOrDefaultAsync(p => p.SKU == sku, cancellationToken);

    public async Task<IReadOnlyList<Product>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
        => await _dbSet.Where(p => p.CategoryId == categoryId && p.IsActive).Include(p => p.Category).ToListAsync(cancellationToken);
}

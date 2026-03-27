using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Data.Repositories;

/// <summary>
/// Repository for <see cref="Category"/> entities. Adds look-up by name
/// and a query for active categories sorted alphabetically.
/// </summary>
public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    /// <inheritdoc />
    public CategoryRepository(ApplicationDbContext context) : base(context) { }

    /// <summary>
    /// Finds a category by its exact <paramref name="name"/>. Returns <c>null</c> if not found.
    /// </summary>
    public async Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        => await _dbSet.FirstOrDefaultAsync(c => c.Name == name, cancellationToken);

    /// <summary>
    /// Returns all active categories ordered alphabetically by name.
    /// </summary>
    public async Task<IReadOnlyList<Category>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default)
        => await _dbSet.Where(c => c.IsActive).OrderBy(c => c.Name).ToListAsync(cancellationToken);
}

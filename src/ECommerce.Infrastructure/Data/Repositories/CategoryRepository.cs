using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Data.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        => await _dbSet.FirstOrDefaultAsync(c => c.Name == name, cancellationToken);

    public async Task<IReadOnlyList<Category>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default)
        => await _dbSet.Where(c => c.IsActive).OrderBy(c => c.Name).ToListAsync(cancellationToken);
}

using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Data.Repositories;

public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    public CustomerRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await _dbSet.FirstOrDefaultAsync(c => c.Email == email, cancellationToken);

    public async Task<Customer?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        => await _dbSet.FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

    public async Task<IReadOnlyList<Customer>> GetActiveCustomersAsync(CancellationToken cancellationToken = default)
        => await _dbSet.Where(c => c.IsActive).OrderBy(c => c.LastName).ThenBy(c => c.FirstName).ToListAsync(cancellationToken);
}

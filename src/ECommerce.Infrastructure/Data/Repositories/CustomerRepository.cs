using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Data.Repositories;

/// <summary>
/// Repository for <see cref="Customer"/> entities. Supports look-up by email or
/// Identity user ID and retrieval of active customers sorted by last name.
/// </summary>
public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    /// <inheritdoc />
    public CustomerRepository(ApplicationDbContext context) : base(context) { }

    /// <summary>
    /// Finds a customer by their email address. Returns <c>null</c> if not found.
    /// </summary>
    public async Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await _dbSet.FirstOrDefaultAsync(c => c.Email == email, cancellationToken);

    /// <summary>
    /// Finds a customer by their linked ASP.NET Identity <paramref name="userId"/>.
    /// Returns <c>null</c> if the customer has not been linked to an identity account.
    /// </summary>
    public async Task<Customer?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        => await _dbSet.FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

    /// <summary>
    /// Returns all active customers ordered by last name, then first name.
    /// </summary>
    public async Task<IReadOnlyList<Customer>> GetActiveCustomersAsync(CancellationToken cancellationToken = default)
        => await _dbSet.Where(c => c.IsActive).OrderBy(c => c.LastName).ThenBy(c => c.FirstName).ToListAsync(cancellationToken);
}

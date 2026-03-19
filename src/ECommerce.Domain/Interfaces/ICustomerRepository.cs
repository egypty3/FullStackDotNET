using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Customer?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Customer>> GetActiveCustomersAsync(CancellationToken cancellationToken = default);
}

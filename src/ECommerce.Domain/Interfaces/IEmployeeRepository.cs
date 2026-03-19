using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

public interface IEmployeeRepository : IRepository<Employee>
{
    Task<Employee?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Employee>> GetByDepartmentAsync(string department, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Employee>> GetActiveEmployeesAsync(CancellationToken cancellationToken = default);
}

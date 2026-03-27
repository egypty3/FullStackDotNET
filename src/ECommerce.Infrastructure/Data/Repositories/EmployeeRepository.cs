using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Data.Repositories;

/// <summary>
/// Repository for <see cref="Employee"/> entities. Supports look-up by email,
/// filtering by department, and retrieval of active employees grouped by department.
/// </summary>
public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
{
    /// <inheritdoc />
    public EmployeeRepository(ApplicationDbContext context) : base(context) { }

    /// <summary>
    /// Finds an employee by their email address. Returns <c>null</c> if not found.
    /// </summary>
    public async Task<Employee?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await _dbSet.FirstOrDefaultAsync(e => e.Email == email, cancellationToken);

    /// <summary>
    /// Returns all active employees in the specified <paramref name="department"/>, ordered by last name.
    /// </summary>
    public async Task<IReadOnlyList<Employee>> GetByDepartmentAsync(string department, CancellationToken cancellationToken = default)
        => await _dbSet.Where(e => e.Department == department && e.IsActive).OrderBy(e => e.LastName).ToListAsync(cancellationToken);

    /// <summary>
    /// Returns all active employees ordered first by department and then by last name.
    /// </summary>
    public async Task<IReadOnlyList<Employee>> GetActiveEmployeesAsync(CancellationToken cancellationToken = default)
        => await _dbSet.Where(e => e.IsActive).OrderBy(e => e.Department).ThenBy(e => e.LastName).ToListAsync(cancellationToken);
}

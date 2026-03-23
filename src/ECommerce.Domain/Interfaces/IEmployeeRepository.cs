using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

/// <summary>
/// Specialized repository interface for Employee entities.
/// Extends the generic IRepository with Employee-specific query methods.
/// 
/// Provides queries for HR and admin features: finding employees by email,
/// filtering by department, and listing active staff members.
/// </summary>
public interface IEmployeeRepository : IRepository<Employee>
{
    /// <summary>
    /// Finds an employee by their work email address. Returns null if not found.
    /// Used for employee lookups and ensuring email uniqueness during creation.
    /// </summary>
    /// <param name="email">The email address to search for.</param>
    /// <param name="cancellationToken">Token to cancel the async operation.</param>
    /// <returns>The matching Employee, or null if not found.</returns>
    Task<Employee?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all employees in a specific department.
    /// Used for department-level reporting, team views, and organizational queries.
    /// </summary>
    /// <param name="department">The department name to filter by (e.g., "Sales", "Engineering").</param>
    /// <param name="cancellationToken">Token to cancel the async operation.</param>
    /// <returns>A read-only list of employees in the specified department.</returns>
    Task<IReadOnlyList<Employee>> GetByDepartmentAsync(string department, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all employees where IsActive is true (current staff, excluding terminated employees).
    /// Used for admin dashboards and active workforce reporting.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the async operation.</param>
    /// <returns>A read-only list of active employees.</returns>
    Task<IReadOnlyList<Employee>> GetActiveEmployeesAsync(CancellationToken cancellationToken = default);
}

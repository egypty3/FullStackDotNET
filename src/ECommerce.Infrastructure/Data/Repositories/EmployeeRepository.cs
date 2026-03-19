using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Data.Repositories;

public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Employee?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await _dbSet.FirstOrDefaultAsync(e => e.Email == email, cancellationToken);

    public async Task<IReadOnlyList<Employee>> GetByDepartmentAsync(string department, CancellationToken cancellationToken = default)
        => await _dbSet.Where(e => e.Department == department && e.IsActive).OrderBy(e => e.LastName).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Employee>> GetActiveEmployeesAsync(CancellationToken cancellationToken = default)
        => await _dbSet.Where(e => e.IsActive).OrderBy(e => e.Department).ThenBy(e => e.LastName).ToListAsync(cancellationToken);
}

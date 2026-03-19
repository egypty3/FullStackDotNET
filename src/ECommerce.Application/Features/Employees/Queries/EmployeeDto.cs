namespace ECommerce.Application.Features.Employees.Queries;

public record EmployeeDto
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string Department { get; init; } = string.Empty;
    public string Position { get; init; } = string.Empty;
    public DateTime HireDate { get; init; }
    public decimal Salary { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
}

using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities;

/// <summary>
/// Represents an employee (staff member) of the ECommerce company.
/// Employees may have various roles such as customer support, warehouse workers, or administrators.
/// 
/// Key design aspects:
/// - Tracks HR-related information: department, position, hire date, and salary.
/// - FullName is a computed property that concatenates first and last name.
/// - Validates that salary cannot be negative (business invariant).
/// - Supports soft-delete via IsActive flag (e.g., for terminated employees whose records need to be preserved).
/// 
/// Inherits Id, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy from BaseEntity.
/// </summary>
public class Employee : BaseEntity
{
    /// <summary>Employee's first name (required for creation).</summary>
    public string FirstName { get; private set; } = string.Empty;

    /// <summary>Employee's last name / family name.</summary>
    public string LastName { get; private set; } = string.Empty;

    /// <summary>Employee's work email address (required for creation).</summary>
    public string Email { get; private set; } = string.Empty;

    /// <summary>Employee's phone number for work-related communication.</summary>
    public string Phone { get; private set; } = string.Empty;

    /// <summary>The department the employee belongs to (e.g., "Sales", "Engineering", "Support").</summary>
    public string Department { get; private set; } = string.Empty;

    /// <summary>The employee's job title/position (e.g., "Senior Developer", "Warehouse Manager").</summary>
    public string Position { get; private set; } = string.Empty;

    /// <summary>The date the employee was hired. Used for seniority calculations and HR reporting.</summary>
    public DateTime HireDate { get; private set; }

    /// <summary>The employee's salary. Validated to be non-negative during creation. Uses decimal for financial precision.</summary>
    public decimal Salary { get; private set; }

    /// <summary>Soft-delete flag. Inactive employees are considered terminated but their records are preserved.</summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>Computed property returning the employee's full display name ("FirstName LastName").</summary>
    public string FullName => $"{FirstName} {LastName}";

    /// <summary>Private parameterless constructor required by EF Core for database materialization.</summary>
    private Employee() { }

    /// <summary>
    /// Factory method to create a new Employee with validation.
    /// Validates that first name and email are provided and that salary is non-negative.
    /// </summary>
    /// <param name="firstName">Employee's first name (required).</param>
    /// <param name="lastName">Employee's last name.</param>
    /// <param name="email">Employee's email (required).</param>
    /// <param name="phone">Employee's phone number.</param>
    /// <param name="department">The department the employee is assigned to.</param>
    /// <param name="position">The employee's job title.</param>
    /// <param name="hireDate">The date the employee was hired.</param>
    /// <param name="salary">The employee's salary (must be >= 0).</param>
    /// <returns>A new Employee instance.</returns>
    /// <exception cref="ArgumentException">Thrown if firstName/email is empty or salary is negative.</exception>
    public static Employee Create(string firstName, string lastName, string email, string phone,
        string department, string position, DateTime hireDate, decimal salary)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required.");
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.");
        if (salary < 0)
            throw new ArgumentException("Salary cannot be negative.");

        return new Employee
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Phone = phone,
            Department = department,
            Position = position,
            HireDate = hireDate,
            Salary = salary
        };
    }

    /// <summary>
    /// Updates the employee's profile and job-related information.
    /// Note: HireDate is not updatable — it's set once during creation as it's a historical fact.
    /// </summary>
    /// <param name="firstName">Updated first name (required).</param>
    /// <param name="lastName">Updated last name.</param>
    /// <param name="email">Updated email address.</param>
    /// <param name="phone">Updated phone number.</param>
    /// <param name="department">Updated department assignment.</param>
    /// <param name="position">Updated job title.</param>
    /// <param name="salary">Updated salary.</param>
    /// <exception cref="ArgumentException">Thrown if firstName is null/whitespace.</exception>
    public void Update(string firstName, string lastName, string email, string phone,
        string department, string position, decimal salary)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required.");

        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        Department = department;
        Position = position;
        Salary = salary;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the employee as inactive (soft-delete), typically when an employee leaves the company.
    /// Their historical data is preserved for auditing and reporting.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Restores a previously deactivated employee (e.g., rehire scenario).
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
}

using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities;

public class Employee : BaseEntity
{
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public string Department { get; private set; } = string.Empty;
    public string Position { get; private set; } = string.Empty;
    public DateTime HireDate { get; private set; }
    public decimal Salary { get; private set; }
    public bool IsActive { get; private set; } = true;

    public string FullName => $"{FirstName} {LastName}";

    private Employee() { }

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

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
}

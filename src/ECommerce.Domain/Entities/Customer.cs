using ECommerce.Domain.Common;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Domain.Entities;

public class Customer : BaseEntity
{
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public Address? ShippingAddress { get; private set; }
    public DateTime? DateOfBirth { get; private set; }
    public bool IsActive { get; private set; } = true;
    public string? UserId { get; private set; }

    public string FullName => $"{FirstName} {LastName}";

    private Customer() { }

    public static Customer Create(string firstName, string lastName, string email, string phone,
        Address? shippingAddress = null, DateTime? dateOfBirth = null, string? userId = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required.");
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.");

        return new Customer
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Phone = phone,
            ShippingAddress = shippingAddress,
            DateOfBirth = dateOfBirth,
            UserId = userId
        };
    }

    public void Update(string firstName, string lastName, string email, string phone,
        Address? shippingAddress, DateTime? dateOfBirth)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required.");

        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        ShippingAddress = shippingAddress;
        DateOfBirth = dateOfBirth;
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

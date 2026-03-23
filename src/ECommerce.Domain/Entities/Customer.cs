using ECommerce.Domain.Common;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Domain.Entities;

/// <summary>
/// Represents a customer who can browse products, place orders, and write reviews.
/// 
/// Key design aspects:
/// - Uses the Address value object for the shipping address (instead of raw string fields),
///   ensuring address validation and consistency.
/// - Has an optional UserId property to link the customer profile to an authentication identity
///   (e.g., ASP.NET Identity user), enabling the separation of business data from auth data.
/// - FullName is a computed property that concatenates first and last name.
/// - Supports soft-delete via IsActive flag.
/// 
/// Inherits Id, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy from BaseEntity.
/// </summary>
public class Customer : BaseEntity
{
    /// <summary>Customer's first name (required for creation).</summary>
    public string FirstName { get; private set; } = string.Empty;

    /// <summary>Customer's last name / family name.</summary>
    public string LastName { get; private set; } = string.Empty;

    /// <summary>Customer's email address (required for creation). Used for order confirmations and communication.</summary>
    public string Email { get; private set; } = string.Empty;

    /// <summary>Customer's phone number for delivery-related communication.</summary>
    public string Phone { get; private set; } = string.Empty;

    /// <summary>
    /// The customer's default shipping address, stored as an Address value object.
    /// Nullable because a customer might register before providing an address.
    /// Can be overridden per-order at checkout time.
    /// </summary>
    public Address? ShippingAddress { get; private set; }

    /// <summary>Customer's date of birth. Nullable and optional — can be used for age verification or birthday promotions.</summary>
    public DateTime? DateOfBirth { get; private set; }

    /// <summary>Soft-delete flag. Inactive customers cannot place orders but their historical data is preserved.</summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>
    /// Optional link to an ASP.NET Identity user ID.
    /// Separates the customer business entity from the authentication identity,
    /// allowing the same customer to exist without a login account (e.g., guest checkout)
    /// or to link later when they create an account.
    /// </summary>
    public string? UserId { get; private set; }

    /// <summary>Computed property returning the customer's full display name ("FirstName LastName").</summary>
    public string FullName => $"{FirstName} {LastName}";

    /// <summary>Private parameterless constructor required by EF Core for database materialization.</summary>
    private Customer() { }

    /// <summary>
    /// Factory method to create a new Customer with validation.
    /// Requires at minimum a first name and email address.
    /// </summary>
    /// <param name="firstName">Customer's first name (required).</param>
    /// <param name="lastName">Customer's last name.</param>
    /// <param name="email">Customer's email (required).</param>
    /// <param name="phone">Customer's phone number.</param>
    /// <param name="shippingAddress">Optional default shipping address.</param>
    /// <param name="dateOfBirth">Optional date of birth.</param>
    /// <param name="userId">Optional Identity user ID to link this customer to an auth account.</param>
    /// <returns>A new Customer instance.</returns>
    /// <exception cref="ArgumentException">Thrown if firstName or email is null/whitespace.</exception>
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

    /// <summary>
    /// Updates the customer's profile information.
    /// Validates that the first name is not empty.
    /// </summary>
    /// <param name="firstName">Updated first name (required).</param>
    /// <param name="lastName">Updated last name.</param>
    /// <param name="email">Updated email address.</param>
    /// <param name="phone">Updated phone number.</param>
    /// <param name="shippingAddress">Updated shipping address (null to clear it).</param>
    /// <param name="dateOfBirth">Updated date of birth.</param>
    /// <exception cref="ArgumentException">Thrown if firstName is null/whitespace.</exception>
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

    /// <summary>
    /// Soft-deletes the customer by setting IsActive to false.
    /// Historical orders and reviews remain intact for reporting purposes.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Restores a previously deactivated customer account.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
}

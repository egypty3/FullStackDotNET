namespace ECommerce.Domain.ValueObjects;

/// <summary>
/// Value Object representing a monetary amount with its associated currency.
/// 
/// Implemented as a C# record (immutable reference type with value-based equality).
/// Two Money instances are considered equal if they have the same Amount and Currency,
/// regardless of being different object references — this is the hallmark of a Value Object in DDD.
/// 
/// Key design decisions:
/// - Immutability: All properties use 'init' accessors, so once created, a Money instance cannot be changed.
/// - Self-validation: The constructor rejects negative amounts and missing currency codes.
/// - Currency safety: Operations like Add() enforce that both operands share the same currency
///   to prevent accidental mixing of different currencies (e.g., adding USD to EUR).
/// - Used by entities like Product (Price), Order (TotalAmount), OrderItem (UnitPrice), and Payment (Amount).
/// </summary>
public record Money
{
    /// <summary>
    /// The numeric monetary value. Guaranteed to be non-negative by constructor validation.
    /// Uses decimal type for precision (avoids floating-point rounding errors common with double/float).
    /// </summary>
    public decimal Amount { get; init; }

    /// <summary>
    /// The ISO 4217 currency code (e.g., "USD", "EUR", "GBP").
    /// Stored in uppercase (normalized via ToUpperInvariant in the constructor).
    /// Defaults to "USD" if not specified.
    /// </summary>
    public string Currency { get; init; } = "USD";

    /// <summary>
    /// Creates a new Money value object with validation.
    /// </summary>
    /// <param name="amount">The monetary amount (must be >= 0).</param>
    /// <param name="currency">The ISO currency code (defaults to "USD"). Stored in uppercase.</param>
    /// <exception cref="ArgumentException">Thrown if amount is negative or currency is empty/whitespace.</exception>
    public Money(decimal amount, string currency = "USD")
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative.", nameof(amount));
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency is required.", nameof(currency));

        Amount = amount;
        // Normalize currency to uppercase for consistent comparison (e.g., "usd" becomes "USD")
        Currency = currency.ToUpperInvariant();
    }

    /// <summary>
    /// Adds two Money values together. Both must have the same currency.
    /// Returns a new Money instance (preserves immutability — the original is not modified).
    /// </summary>
    /// <param name="other">The Money value to add to this one.</param>
    /// <returns>A new Money instance with the summed amount.</returns>
    /// <exception cref="InvalidOperationException">Thrown if currencies don't match (e.g., adding USD to EUR).</exception>
    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot add different currencies.");
        return new Money(Amount + other.Amount, Currency);
    }

    /// <summary>
    /// Multiplies this Money value by a quantity (e.g., unit price x quantity for order item subtotals).
    /// Returns a new Money instance (preserves immutability).
    /// </summary>
    /// <param name="quantity">The multiplier (e.g., number of items ordered).</param>
    /// <returns>A new Money instance with Amount = original amount * quantity.</returns>
    public Money Multiply(int quantity)
    {
        return new Money(Amount * quantity, Currency);
    }

    /// <summary>
    /// Returns a formatted string representation of the monetary value (e.g., "29.99 USD").
    /// Uses F2 format specifier to always show exactly 2 decimal places.
    /// </summary>
    public override string ToString() => $"{Amount:F2} {Currency}";
}

/// <summary>
/// Value Object representing a physical/mailing address.
/// 
/// Implemented as a C# record for immutability and value-based equality.
/// Two Address instances with the same street, city, state, zip, and country are considered equal.
/// 
/// Used by the Customer entity (ShippingAddress) and Order entity (ShippingAddress)
/// to store delivery location information.
/// 
/// Self-validates that required fields (Street, City, Country) are provided.
/// State and ZipCode are not validated as required because some countries don't use them.
/// </summary>
public record Address
{
    /// <summary>Street address line (e.g., "123 Main St, Apt 4B").</summary>
    public string Street { get; init; } = string.Empty;

    /// <summary>City or municipality name.</summary>
    public string City { get; init; } = string.Empty;

    /// <summary>State, province, or region. Not required as some countries don't have states.</summary>
    public string State { get; init; } = string.Empty;

    /// <summary>Postal/ZIP code. Not required as some countries don't use postal codes.</summary>
    public string ZipCode { get; init; } = string.Empty;

    /// <summary>Country name or ISO country code.</summary>
    public string Country { get; init; } = string.Empty;

    /// <summary>
    /// Creates a new Address value object with validation of required fields.
    /// </summary>
    /// <param name="street">Street address (required).</param>
    /// <param name="city">City name (required).</param>
    /// <param name="state">State or province (optional — not all countries have states).</param>
    /// <param name="zipCode">Postal/ZIP code (optional — not all countries use postal codes).</param>
    /// <param name="country">Country name (required).</param>
    /// <exception cref="ArgumentException">Thrown if street, city, or country is empty/whitespace.</exception>
    public Address(string street, string city, string state, string zipCode, string country)
    {
        if (string.IsNullOrWhiteSpace(street)) throw new ArgumentException("Street is required.");
        if (string.IsNullOrWhiteSpace(city)) throw new ArgumentException("City is required.");
        if (string.IsNullOrWhiteSpace(country)) throw new ArgumentException("Country is required.");

        Street = street;
        City = city;
        State = state;
        ZipCode = zipCode;
        Country = country;
    }
}

namespace ECommerce.Application.Features.Customers.Queries;

public record CustomerDto
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string? ShippingStreet { get; init; }
    public string? ShippingCity { get; init; }
    public string? ShippingState { get; init; }
    public string? ShippingZipCode { get; init; }
    public string? ShippingCountry { get; init; }
    public DateTime? DateOfBirth { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
}

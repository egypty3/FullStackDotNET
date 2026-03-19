namespace ECommerce.Application.Features.Shippers.Queries;

public record ShipperDto
{
    public Guid Id { get; init; }
    public string CompanyName { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Website { get; init; }
    public string ContactPerson { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
}

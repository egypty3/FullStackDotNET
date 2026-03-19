using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities;

public class Shipper : BaseEntity
{
    public string CompanyName { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? Website { get; private set; }
    public string ContactPerson { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;

    private Shipper() { }

    public static Shipper Create(string companyName, string phone, string email,
        string contactPerson, string? website = null)
    {
        if (string.IsNullOrWhiteSpace(companyName))
            throw new ArgumentException("Company name is required.");
        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Phone is required.");

        return new Shipper
        {
            CompanyName = companyName,
            Phone = phone,
            Email = email,
            ContactPerson = contactPerson,
            Website = website
        };
    }

    public void Update(string companyName, string phone, string email,
        string contactPerson, string? website)
    {
        if (string.IsNullOrWhiteSpace(companyName))
            throw new ArgumentException("Company name is required.");

        CompanyName = companyName;
        Phone = phone;
        Email = email;
        ContactPerson = contactPerson;
        Website = website;
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

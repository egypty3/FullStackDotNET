using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities;

/// <summary>
/// Represents a shipping carrier / logistics company that handles order deliveries
/// (e.g., FedEx, UPS, DHL, or a local courier service).
/// 
/// Shippers are referenced by Shipment entities to track which carrier is handling each delivery.
/// 
/// Key design aspects:
/// - Stores company contact information (phone, email, website, contact person).
/// - Supports soft-delete via IsActive flag so deactivated carriers can't be assigned to new shipments
///   while preserving historical shipment records that reference them.
/// 
/// Inherits Id, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy from BaseEntity.
/// </summary>
public class Shipper : BaseEntity
{
    /// <summary>The carrier company's name (e.g., "FedEx", "UPS"). Required for creation.</summary>
    public string CompanyName { get; private set; } = string.Empty;

    /// <summary>The carrier's contact phone number. Required for creation.</summary>
    public string Phone { get; private set; } = string.Empty;

    /// <summary>The carrier's contact email address.</summary>
    public string Email { get; private set; } = string.Empty;

    /// <summary>Optional URL to the carrier's website (e.g., for tracking page links).</summary>
    public string? Website { get; private set; }

    /// <summary>Name of the primary contact person at the carrier company.</summary>
    public string ContactPerson { get; private set; } = string.Empty;

    /// <summary>
    /// Soft-delete flag. Inactive shippers cannot be assigned to new shipments
    /// but historical shipment records that reference them are preserved.
    /// </summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>Private parameterless constructor required by EF Core for database materialization.</summary>
    private Shipper() { }

    /// <summary>
    /// Factory method to create a new Shipper with validation.
    /// Requires a company name and phone number at minimum.
    /// </summary>
    /// <param name="companyName">The carrier's company name (required).</param>
    /// <param name="phone">The carrier's phone number (required).</param>
    /// <param name="email">The carrier's email address.</param>
    /// <param name="contactPerson">The name of the primary contact at the carrier.</param>
    /// <param name="website">Optional website URL.</param>
    /// <returns>A new active Shipper instance.</returns>
    /// <exception cref="ArgumentException">Thrown if companyName or phone is empty.</exception>
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

    /// <summary>
    /// Updates the shipper's contact and company information.
    /// Validates that the company name is not empty.
    /// </summary>
    /// <param name="companyName">Updated company name (required).</param>
    /// <param name="phone">Updated phone number.</param>
    /// <param name="email">Updated email address.</param>
    /// <param name="contactPerson">Updated contact person name.</param>
    /// <param name="website">Updated website URL (null to remove).</param>
    /// <exception cref="ArgumentException">Thrown if companyName is empty.</exception>
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

    /// <summary>
    /// Deactivates the shipper so it can no longer be assigned to new shipments.
    /// Existing shipments with this shipper are not affected.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Re-activates a previously deactivated shipper, allowing new shipments to be assigned to it.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
}

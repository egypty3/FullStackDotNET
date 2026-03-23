using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities;

/// <summary>
/// Represents a product category in the ECommerce catalog (e.g., "Electronics", "Clothing").
/// Categories are used to organize and group products for browsing and filtering.
/// 
/// Follows the Rich Domain Model pattern:
/// - Private setters enforce encapsulation — properties can only be changed through explicit domain methods.
/// - A private constructor prevents direct instantiation; use the static Create() factory method instead.
/// - Business rules (e.g., "name is required") are validated within the entity itself.
/// - Supports soft-delete via IsActive flag (Deactivate/Activate) rather than hard deletion from the database.
/// 
/// Inherits Id, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy from BaseEntity.
/// </summary>
public class Category : BaseEntity
{
    /// <summary>The display name of the category (e.g., "Electronics"). Cannot be null or whitespace.</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>A detailed description of what products belong in this category.</summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// Soft-delete flag. When false, the category is considered "deleted" or hidden
    /// without actually removing the database record (preserves referential integrity with products).
    /// Defaults to true (active) upon creation.
    /// </summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>
    /// Private parameterless constructor required by EF Core for materialization (creating entity instances
    /// when reading from the database). Not accessible to application code.
    /// </summary>
    private Category() { }

    /// <summary>
    /// Factory method to create a new Category instance with validation.
    /// This is the only way to create a Category — the constructor is private.
    /// 
    /// Using a static factory method (instead of a public constructor) allows:
    /// 1. Input validation before the object is created
    /// 2. A descriptive method name that communicates intent
    /// 3. Future flexibility to return different subtypes if needed
    /// </summary>
    /// <param name="name">The category name (required — cannot be null or whitespace).</param>
    /// <param name="description">A description of the category.</param>
    /// <returns>A new Category instance with the specified name and description.</returns>
    /// <exception cref="ArgumentException">Thrown if name is null or whitespace.</exception>
    public static Category Create(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name is required.");

        return new Category
        {
            Name = name,
            Description = description
        };
    }

    /// <summary>
    /// Updates the category's name and description.
    /// Validates that the new name is not empty and sets the UpdatedAt audit timestamp.
    /// </summary>
    /// <param name="name">The new category name (required).</param>
    /// <param name="description">The new category description.</param>
    /// <exception cref="ArgumentException">Thrown if name is null or whitespace.</exception>
    public void Update(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name is required.");

        Name = name;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Soft-deletes the category by setting IsActive to false.
    /// Products in this category remain in the database but the category won't appear in active listings.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Restores a previously deactivated category by setting IsActive back to true.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
}

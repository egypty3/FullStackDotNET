using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

/// <summary>
/// Specialized repository interface for Category entities.
/// Extends the generic IRepository with Category-specific query methods.
/// 
/// Inherits all standard CRUD operations from IRepository{Category}
/// and adds domain-specific queries for common category access patterns.
/// </summary>
public interface ICategoryRepository : IRepository<Category>
{
    /// <summary>
    /// Finds a category by its exact name. Returns null if no category with that name exists.
    /// Useful for checking uniqueness before creating a new category.
    /// </summary>
    /// <param name="name">The category name to search for.</param>
    /// <param name="cancellationToken">Token to cancel the async operation.</param>
    /// <returns>The matching Category, or null if not found.</returns>
    Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all categories where IsActive is true.
    /// Used for populating category dropdowns, navigation menus, and catalog filters
    /// — only showing categories that are currently available.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the async operation.</param>
    /// <returns>A read-only list of active categories.</returns>
    Task<IReadOnlyList<Category>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default);
}

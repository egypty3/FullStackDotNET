using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

/// <summary>
/// Specialized repository interface for Customer entities.
/// Extends the generic IRepository with Customer-specific query methods.
/// 
/// Provides lookups by email (for registration/login flows) and by Identity userId
/// (for linking authenticated users to their customer profiles).
/// </summary>
public interface ICustomerRepository : IRepository<Customer>
{
    /// <summary>
    /// Finds a customer by their email address. Returns null if no customer with that email exists.
    /// Used during registration to check for existing accounts, and for email-based lookups.
    /// </summary>
    /// <param name="email">The email address to search for.</param>
    /// <param name="cancellationToken">Token to cancel the async operation.</param>
    /// <returns>The matching Customer, or null if not found.</returns>
    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds a customer by their ASP.NET Identity user ID.
    /// Used after authentication to load the customer profile linked to the logged-in user.
    /// </summary>
    /// <param name="userId">The Identity user ID string.</param>
    /// <param name="cancellationToken">Token to cancel the async operation.</param>
    /// <returns>The matching Customer, or null if no customer is linked to this user ID.</returns>
    Task<Customer?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all customers where IsActive is true.
    /// Used by admin dashboards and CRM features to list currently active customer accounts.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the async operation.</param>
    /// <returns>A read-only list of active customers.</returns>
    Task<IReadOnlyList<Customer>> GetActiveCustomersAsync(CancellationToken cancellationToken = default);
}

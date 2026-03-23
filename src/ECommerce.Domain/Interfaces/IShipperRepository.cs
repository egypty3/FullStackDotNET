using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

/// <summary>
/// Specialized repository interface for Shipper (carrier company) entities.
/// Extends the generic IRepository with Shipper-specific query methods.
/// 
/// Provides queries for managing shipping partners: finding carriers by name
/// and listing active carriers available for new shipments.
/// </summary>
public interface IShipperRepository : IRepository<Shipper>
{
    /// <summary>
    /// Finds a shipper by its company name. Returns null if no shipper with that name exists.
    /// Used for name-based lookups and ensuring uniqueness of carrier company names.
    /// </summary>
    /// <param name="companyName">The carrier company name to search for.</param>
    /// <param name="cancellationToken">Token to cancel the async operation.</param>
    /// <returns>The matching Shipper, or null if not found.</returns>
    Task<Shipper?> GetByNameAsync(string companyName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all shippers where IsActive is true.
    /// Used for populating carrier selection dropdowns when creating new shipments —
    /// only active carriers can be assigned to new deliveries.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the async operation.</param>
    /// <returns>A read-only list of active shippers.</returns>
    Task<IReadOnlyList<Shipper>> GetActiveShippersAsync(CancellationToken cancellationToken = default);
}

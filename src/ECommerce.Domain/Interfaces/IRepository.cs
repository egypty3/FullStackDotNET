using ECommerce.Domain.Common;
using System.Linq.Expressions;

namespace ECommerce.Domain.Interfaces;

/// <summary>
/// Generic repository interface defining the standard CRUD operations for all domain entities.
/// 
/// This is the base contract for all repositories in the application, following the Repository Pattern.
/// Concrete implementations live in the Infrastructure layer (e.g., EfRepository{T} using EF Core).
/// 
/// Design principles applied:
/// - Interface Segregation Principle (ISP): This base interface defines only common operations.
///   Entity-specific operations are defined in specialized interfaces (e.g., IProductRepository).
/// - Dependency Inversion Principle (DIP): The Domain layer defines the interface; the Infrastructure
///   layer provides the implementation. This keeps the Domain free of persistence concerns.
/// 
/// The generic constraint "where T : BaseEntity" ensures only domain entities (with Id, timestamps, etc.)
/// can be used with this repository.
/// 
/// All methods are async and accept an optional CancellationToken for cooperative cancellation
/// (e.g., when an HTTP request is aborted, in-progress database queries can be cancelled).
/// </summary>
/// <typeparam name="T">The domain entity type. Must inherit from BaseEntity.</typeparam>
public interface IRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Retrieves a single entity by its unique identifier.
    /// Returns null if no entity with the given ID exists.
    /// </summary>
    /// <param name="id">The GUID identifier of the entity.</param>
    /// <param name="cancellationToken">Token to cancel the async operation.</param>
    /// <returns>The entity if found, or null.</returns>
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all entities of type T from the data store.
    /// Returns an IReadOnlyList to prevent modification of the returned collection.
    /// Use with caution on large tables — consider using FindAsync with a predicate for filtering.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the async operation.</param>
    /// <returns>A read-only list of all entities.</returns>
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds entities matching a given predicate (filter expression).
    /// The predicate is an Expression tree (not a compiled Func) so it can be translated
    /// to SQL by EF Core, enabling server-side filtering instead of loading all records.
    /// 
    /// Example: repo.FindAsync(p => p.IsActive && p.Price.Amount > 10)
    /// </summary>
    /// <param name="predicate">A LINQ expression defining the filter criteria.</param>
    /// <param name="cancellationToken">Token to cancel the async operation.</param>
    /// <returns>A read-only list of entities matching the predicate.</returns>
    Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new entity to the data store.
    /// The entity is tracked by the ORM but not persisted until SaveChangesAsync is called on the UnitOfWork.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">Token to cancel the async operation.</param>
    /// <returns>The added entity (with any ORM-generated values).</returns>
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks an existing entity as modified in the ORM change tracker.
    /// Changes are not persisted until SaveChangesAsync is called on the UnitOfWork.
    /// </summary>
    /// <param name="entity">The entity with updated property values.</param>
    /// <param name="cancellationToken">Token to cancel the async operation.</param>
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks an entity for deletion from the data store.
    /// The deletion is not persisted until SaveChangesAsync is called on the UnitOfWork.
    /// Note: Many entities use soft-delete (IsActive flag) instead of hard deletion.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <param name="cancellationToken">Token to cancel the async operation.</param>
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether an entity with the given ID exists in the data store.
    /// More efficient than GetByIdAsync when you only need to verify existence (no data transfer).
    /// </summary>
    /// <param name="id">The GUID identifier to check.</param>
    /// <param name="cancellationToken">Token to cancel the async operation.</param>
    /// <returns>True if an entity with the given ID exists; otherwise false.</returns>
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the total count of entities of type T in the data store.
    /// Useful for pagination calculations and dashboard statistics.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the async operation.</param>
    /// <returns>The total number of entities.</returns>
    Task<int> CountAsync(CancellationToken cancellationToken = default);
}

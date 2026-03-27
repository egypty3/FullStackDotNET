using ECommerce.Domain.Common;
using ECommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ECommerce.Infrastructure.Data.Repositories;

/// <summary>
/// Generic base repository that implements <see cref="IRepository{T}"/> for any entity
/// derived from <see cref="BaseEntity"/>. Provides standard CRUD operations backed by
/// an EF Core <see cref="DbSet{T}"/>.
/// <para>
/// Follows the Liskov Substitution Principle — any subclass can be used wherever
/// <see cref="IRepository{T}"/> is expected without changing behaviour.
/// Subclasses (e.g., <c>ProductRepository</c>) override virtual methods to add
/// eager-loading or domain-specific queries.
/// </para>
/// </summary>
/// <typeparam name="T">The domain entity type; must inherit from <see cref="BaseEntity"/>.</typeparam>
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    /// <summary>The shared EF Core database context.</summary>
    protected readonly ApplicationDbContext _context;

    /// <summary>The <see cref="DbSet{T}"/> for the entity type <typeparamref name="T"/>.</summary>
    protected readonly DbSet<T> _dbSet;

    /// <summary>
    /// Initializes a new instance of <see cref="Repository{T}"/>.
    /// </summary>
    /// <param name="context">The EF Core <see cref="ApplicationDbContext"/> to operate against.</param>
    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    /// <summary>
    /// Retrieves a single entity by its primary key. Returns <c>null</c> if not found.
    /// Marked <c>virtual</c> so subclasses can override to add <c>Include</c> calls.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>The entity if found; otherwise <c>null</c>.</returns>
    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbSet.FindAsync(new object[] { id }, cancellationToken);

    /// <summary>
    /// Retrieves all entities of type <typeparamref name="T"/> from the database.
    /// </summary>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A read-only list of all entities.</returns>
    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _dbSet.ToListAsync(cancellationToken);

    /// <summary>
    /// Retrieves all entities that match the given predicate expression.
    /// </summary>
    /// <param name="predicate">A LINQ expression used to filter entities.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A read-only list of matching entities.</returns>
    public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        => await _dbSet.Where(predicate).ToListAsync(cancellationToken);

    /// <summary>
    /// Adds a new entity to the change tracker. The entity is not persisted until
    /// <see cref="IUnitOfWork.SaveChangesAsync"/> is called.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>The same entity instance (now tracked by EF Core).</returns>
    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    /// <summary>
    /// Marks an existing entity as modified so its changes will be persisted on the next save.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _context.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    /// <summary>
    /// Removes an entity from the change tracker so it will be deleted on the next save.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    public Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Checks whether an entity with the specified <paramref name="id"/> exists in the database.
    /// </summary>
    /// <param name="id">The unique identifier to look up.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns><c>true</c> if the entity exists; otherwise <c>false</c>.</returns>
    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbSet.AnyAsync(e => e.Id == id, cancellationToken);

    /// <summary>
    /// Returns the total number of entities of type <typeparamref name="T"/> in the database.
    /// </summary>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>The entity count.</returns>
    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        => await _dbSet.CountAsync(cancellationToken);
}

namespace ECommerce.Domain.Common;

/// <summary>
/// Abstract base class for all domain entities in the ECommerce system.
/// Provides common audit trail properties (Id, timestamps, user tracking) that every entity inherits.
/// 
/// This class follows the DDD (Domain-Driven Design) pattern where all entities share a common identity
/// and auditing mechanism. By making the class abstract, it cannot be instantiated directly — only
/// concrete entity subclasses (e.g., Product, Order, Customer) can be created.
/// 
/// The Id property uses a GUID for globally unique identification, which avoids database-generated
/// sequential IDs and supports distributed systems where multiple databases might create records independently.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Unique identifier for the entity, automatically generated as a new GUID upon creation.
    /// Uses a protected setter so only the entity itself or derived classes can modify it
    /// (prevents external code from changing an entity's identity).
    /// </summary>
    public Guid Id { get; protected set; } = Guid.NewGuid();

    /// <summary>
    /// Timestamp recording when the entity was first created.
    /// Defaults to the current UTC time at object instantiation.
    /// UTC is used to avoid timezone ambiguity across distributed systems.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Timestamp recording the last time the entity was modified.
    /// Nullable because it is null until the entity is updated for the first time.
    /// Updated by domain methods (e.g., Update(), Deactivate()) whenever state changes occur.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Identifier (e.g., user ID or username) of the user who originally created this entity.
    /// Nullable to support system-generated entities or seeded data that may not have a specific creator.
    /// Typically populated by the application layer or middleware using the current authenticated user.
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Identifier of the user who last modified this entity.
    /// Nullable because it remains null until the entity is first updated.
    /// Typically populated by the application layer or middleware using the current authenticated user.
    /// </summary>
    public string? UpdatedBy { get; set; }
}

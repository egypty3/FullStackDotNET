namespace ECommerce.Domain.Interfaces;

/// <summary>
/// Unit of Work interface that coordinates multiple repository operations within a single database transaction.
/// 
/// The Unit of Work pattern ensures that all changes made through multiple repositories are committed
/// or rolled back as a single atomic operation. This prevents partial updates (e.g., creating an order
/// but failing to reduce product stock) which would leave the database in an inconsistent state.
/// 
/// Usage pattern in the Application layer:
///   1. Access repositories via IUnitOfWork properties (e.g., unitOfWork.Products)
///   2. Perform multiple operations across different repositories
///   3. Call SaveChangesAsync() once to persist ALL changes in a single database transaction
/// 
/// Implements IDisposable to ensure the underlying database context/connection is properly released
/// when the Unit of Work goes out of scope (typically managed by the DI container's lifetime scope).
/// 
/// The concrete implementation lives in the Infrastructure layer and wraps EF Core's DbContext,
/// which natively supports the Unit of Work pattern through its change tracking mechanism.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>Repository for Product entity CRUD and specialized queries.</summary>
    IProductRepository Products { get; }

    /// <summary>Repository for Order entity CRUD and specialized queries (e.g., get by customer, get with items).</summary>
    IOrderRepository Orders { get; }

    /// <summary>Repository for Category entity CRUD and specialized queries (e.g., active categories).</summary>
    ICategoryRepository Categories { get; }

    /// <summary>Repository for Customer entity CRUD and specialized queries (e.g., get by email, get by userId).</summary>
    ICustomerRepository Customers { get; }

    /// <summary>Repository for Employee entity CRUD and specialized queries (e.g., get by department).</summary>
    IEmployeeRepository Employees { get; }

    /// <summary>Repository for Shipper entity CRUD and specialized queries (e.g., active shippers).</summary>
    IShipperRepository Shippers { get; }

    /// <summary>Repository for Shipment entity CRUD and specialized queries (e.g., get by tracking number).</summary>
    IShipmentRepository Shipments { get; }

    /// <summary>Repository for Payment entity CRUD and specialized queries (e.g., get by transaction ID).</summary>
    IPaymentRepository Payments { get; }

    /// <summary>Repository for Review entity CRUD and specialized queries (e.g., approved reviews, pending reviews).</summary>
    IReviewRepository Reviews { get; }

    /// <summary>Repository for Coupon entity CRUD and specialized queries (e.g., get by code, active coupons).</summary>
    ICouponRepository Coupons { get; }

    /// <summary>
    /// Persists all pending changes (inserts, updates, deletes) tracked by the Unit of Work
    /// to the database in a single atomic transaction.
    /// 
    /// Returns the number of state entries (rows) written to the database.
    /// If any part of the save fails, the entire transaction is rolled back.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the async operation.</param>
    /// <returns>The number of database rows affected by the save operation.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

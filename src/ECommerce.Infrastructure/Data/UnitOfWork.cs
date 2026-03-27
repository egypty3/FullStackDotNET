using ECommerce.Domain.Interfaces;
using ECommerce.Infrastructure.Data.Repositories;

namespace ECommerce.Infrastructure.Data;

/// <summary>
/// Implements the Unit of Work pattern by coordinating changes across multiple repositories
/// and committing them in a single database transaction through the underlying <see cref="ApplicationDbContext"/>.
/// <para>
/// Each repository is lazily instantiated on first access using the null-coalescing assignment operator,
/// ensuring that only the repositories actually used during a request are created.
/// Implements <see cref="IDisposable"/> to release the <see cref="ApplicationDbContext"/> when the
/// scoped lifetime ends.
/// </para>
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IProductRepository? _products;
    private IOrderRepository? _orders;
    private ICategoryRepository? _categories;
    private ICustomerRepository? _customers;
    private IEmployeeRepository? _employees;
    private IShipperRepository? _shippers;
    private IShipmentRepository? _shipments;
    private IPaymentRepository? _payments;
    private IReviewRepository? _reviews;
    private ICouponRepository? _coupons;

    /// <summary>
    /// Initializes a new instance of <see cref="UnitOfWork"/> with the given database context.
    /// </summary>
    /// <param name="context">The EF Core <see cref="ApplicationDbContext"/> shared by all repositories.</param>
    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>Gets the Product repository, creating it on first access.</summary>
    public IProductRepository Products => _products ??= new ProductRepository(_context);

    /// <summary>Gets the Order repository, creating it on first access.</summary>
    public IOrderRepository Orders => _orders ??= new OrderRepository(_context);

    /// <summary>Gets the Category repository, creating it on first access.</summary>
    public ICategoryRepository Categories => _categories ??= new CategoryRepository(_context);

    /// <summary>Gets the Customer repository, creating it on first access.</summary>
    public ICustomerRepository Customers => _customers ??= new CustomerRepository(_context);

    /// <summary>Gets the Employee repository, creating it on first access.</summary>
    public IEmployeeRepository Employees => _employees ??= new EmployeeRepository(_context);

    /// <summary>Gets the Shipper repository, creating it on first access.</summary>
    public IShipperRepository Shippers => _shippers ??= new ShipperRepository(_context);

    /// <summary>Gets the Shipment repository, creating it on first access.</summary>
    public IShipmentRepository Shipments => _shipments ??= new ShipmentRepository(_context);

    /// <summary>Gets the Payment repository, creating it on first access.</summary>
    public IPaymentRepository Payments => _payments ??= new PaymentRepository(_context);

    /// <summary>Gets the Review repository, creating it on first access.</summary>
    public IReviewRepository Reviews => _reviews ??= new ReviewRepository(_context);

    /// <summary>Gets the Coupon repository, creating it on first access.</summary>
    public ICouponRepository Coupons => _coupons ??= new CouponRepository(_context);

    /// <summary>
    /// Commits all pending changes tracked by the underlying <see cref="ApplicationDbContext"/>
    /// to the database in a single transaction.
    /// </summary>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>The number of state entries written to the database.</returns>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    /// <summary>
    /// Disposes the underlying <see cref="ApplicationDbContext"/> and suppresses finalization
    /// to prevent redundant cleanup by the garbage collector.
    /// </summary>
    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}

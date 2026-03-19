using ECommerce.Domain.Interfaces;
using ECommerce.Infrastructure.Data.Repositories;

namespace ECommerce.Infrastructure.Data;

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

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IProductRepository Products => _products ??= new ProductRepository(_context);
    public IOrderRepository Orders => _orders ??= new OrderRepository(_context);
    public ICategoryRepository Categories => _categories ??= new CategoryRepository(_context);
    public ICustomerRepository Customers => _customers ??= new CustomerRepository(_context);
    public IEmployeeRepository Employees => _employees ??= new EmployeeRepository(_context);
    public IShipperRepository Shippers => _shippers ??= new ShipperRepository(_context);
    public IShipmentRepository Shipments => _shipments ??= new ShipmentRepository(_context);
    public IPaymentRepository Payments => _payments ??= new PaymentRepository(_context);
    public IReviewRepository Reviews => _reviews ??= new ReviewRepository(_context);
    public ICouponRepository Coupons => _coupons ??= new CouponRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}

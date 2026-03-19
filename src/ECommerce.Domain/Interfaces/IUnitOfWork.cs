namespace ECommerce.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IProductRepository Products { get; }
    IOrderRepository Orders { get; }
    ICategoryRepository Categories { get; }
    ICustomerRepository Customers { get; }
    IEmployeeRepository Employees { get; }
    IShipperRepository Shippers { get; }
    IShipmentRepository Shipments { get; }
    IPaymentRepository Payments { get; }
    IReviewRepository Reviews { get; }
    ICouponRepository Coupons { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

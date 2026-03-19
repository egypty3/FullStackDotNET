using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

public interface IShipperRepository : IRepository<Shipper>
{
    Task<Shipper?> GetByNameAsync(string companyName, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Shipper>> GetActiveShippersAsync(CancellationToken cancellationToken = default);
}

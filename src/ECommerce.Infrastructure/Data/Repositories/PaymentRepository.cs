using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using ECommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Data.Repositories;

public class PaymentRepository : Repository<Payment>, IPaymentRepository
{
    public PaymentRepository(ApplicationDbContext context) : base(context) { }

    public override async Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbSet.Include(p => p.Order).FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<Payment?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
        => await _dbSet.Include(p => p.Order).FirstOrDefaultAsync(p => p.OrderId == orderId, cancellationToken);

    public async Task<Payment?> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken = default)
        => await _dbSet.FirstOrDefaultAsync(p => p.TransactionId == transactionId, cancellationToken);

    public async Task<IReadOnlyList<Payment>> GetByStatusAsync(PaymentStatus status, CancellationToken cancellationToken = default)
        => await _dbSet.Where(p => p.Status == status).OrderByDescending(p => p.CreatedAt).ToListAsync(cancellationToken);
}

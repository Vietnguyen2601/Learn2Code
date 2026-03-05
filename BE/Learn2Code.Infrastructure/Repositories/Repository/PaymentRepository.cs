using Learn2Code.Domain.Entities;
using Learn2Code.Infrastructure.Data.Context;
using Learn2Code.Infrastructure.Repositories.Base;
using Learn2Code.Infrastructure.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Learn2Code.Infrastructure.Repositories.Repository;

public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
{
    public PaymentRepository(Learn2CodeDbContext context) : base(context)
    {
    }

    public async Task<List<Payment>> GetByStudentIdAsync(Guid studentId)
    {
        return await _context.Set<Payment>()
            .Include(p => p.Subscription)
            .Where(p => p.Subscription.UserId == studentId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Payment>> GetAllWithDetailsAsync()
    {
        return await _context.Set<Payment>()
            .Include(p => p.Subscription)
                .ThenInclude(s => s.User)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }
}

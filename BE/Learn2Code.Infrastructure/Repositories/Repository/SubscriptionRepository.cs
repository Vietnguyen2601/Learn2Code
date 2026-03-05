using Learn2Code.Domain.Entities;
using Learn2Code.Domain.Enums;
using Learn2Code.Infrastructure.Data.Context;
using Learn2Code.Infrastructure.Repositories.Base;
using Learn2Code.Infrastructure.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Learn2Code.Infrastructure.Repositories.Repository;

public class SubscriptionRepository : GenericRepository<UserSubscription>, ISubscriptionRepository
{
    public SubscriptionRepository(Learn2CodeDbContext context) : base(context)
    {
    }

    /// <summary>Current Active or Pending subscription of a user, with Package loaded.</summary>
    public async Task<UserSubscription?> GetCurrentActiveAsync(Guid userId)
    {
        return await _context.UserSubscriptions
            .Include(s => s.Package)
            .Where(s => s.UserId == userId &&
                        (s.Status == SubscriptionStatus.Active || s.Status == SubscriptionStatus.Pending))
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync();
    }

    /// <summary>Single subscription with Package + Payments loaded, scoped to the owner.</summary>
    public async Task<UserSubscription?> GetByIdWithDetailsAsync(Guid subscriptionId, Guid userId)
    {
        return await _context.UserSubscriptions
            .Include(s => s.Package)
            .Include(s => s.Payments)
            .FirstOrDefaultAsync(s => s.SubscriptionId == subscriptionId && s.UserId == userId);
    }

    /// <summary>All subscriptions with Package loaded, newest first.</summary>
    public async Task<List<UserSubscription>> GetAllWithPackageAsync()
    {
        return await _context.UserSubscriptions
            .Include(s => s.Package)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }
}

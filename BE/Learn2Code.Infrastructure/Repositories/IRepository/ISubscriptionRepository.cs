using Learn2Code.Domain.Entities;
using Learn2Code.Infrastructure.Repositories.Base;

namespace Learn2Code.Infrastructure.Repositories.IRepository;

public interface ISubscriptionRepository : IGenericRepository<UserSubscription>
{
    Task<UserSubscription?> GetCurrentActiveAsync(Guid userId);
    Task<UserSubscription?> GetByIdWithDetailsAsync(Guid subscriptionId, Guid userId);
    Task<List<UserSubscription>> GetAllWithPackageAsync();
}

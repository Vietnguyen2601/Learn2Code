using Learn2Code.Domain.Entities;
using Learn2Code.Infrastructure.Repositories.Base;

namespace Learn2Code.Infrastructure.Repositories.IRepository;

public interface ISubscriptionPackageRepository : IGenericRepository<SubscriptionPackage>
{
    Task<List<SubscriptionPackage>> GetAllActiveOrderedByPriceAsync();
}

using Learn2Code.Domain.Entities;
using Learn2Code.Infrastructure.Data.Context;
using Learn2Code.Infrastructure.Repositories.Base;
using Learn2Code.Infrastructure.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Learn2Code.Infrastructure.Repositories.Repository;

public class SubscriptionPackageRepository : GenericRepository<SubscriptionPackage>, ISubscriptionPackageRepository
{
    public SubscriptionPackageRepository(Learn2CodeDbContext context) : base(context)
    {
    }

    public async Task<List<SubscriptionPackage>> GetAllActiveOrderedByPriceAsync()
    {
        return await _context.SubscriptionPackages
            .Where(p => p.IsActive)
            .OrderBy(p => p.Price)
            .ToListAsync();
    }
}

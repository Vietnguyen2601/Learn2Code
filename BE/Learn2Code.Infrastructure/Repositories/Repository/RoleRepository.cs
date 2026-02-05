using Learn2Code.Infrastructure.Repositories.IRepository;
using Learn2Code.Domain.Entities;
using Learn2Code.Infrastructure.Data.Context;
using Learn2Code.Infrastructure.Repositories.Base;

namespace Learn2Code.Infrastructure.Repositories.Repository;

public class RoleRepository : GenericRepository<Role>, IRoleRepository
{
    public RoleRepository(Learn2CodeDbContext context) : base(context)
    {
    }
}

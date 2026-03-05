using Learn2Code.Domain.Entities;
using Learn2Code.Infrastructure.Repositories.Base;

namespace Learn2Code.Infrastructure.Repositories.IRepository;

public interface IAccountRepository : IGenericRepository<Account>
{
    Task<Account?> GetByEmailOrUsernameWithRolesAsync(string emailOrUsername);
    Task<List<Account>> GetAllWithRolesAsync();
    Task<Account?> GetByIdWithRolesAsync(Guid id);
}

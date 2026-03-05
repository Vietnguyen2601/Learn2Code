using Learn2Code.Infrastructure.Repositories.IRepository;
using Learn2Code.Domain.Entities;
using Learn2Code.Infrastructure.Data.Context;
using Learn2Code.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Learn2Code.Infrastructure.Repositories.Repository;

public class AccountRepository : GenericRepository<Account>, IAccountRepository
{
    public AccountRepository(Learn2CodeDbContext context) : base(context)
    {
    }

    public async Task<Account?> GetByEmailOrUsernameWithRolesAsync(string emailOrUsername)
    {
        return await _context.Set<Account>()
            .Include(a => a.AccountRoles)
            .ThenInclude(ar => ar.Role)
            .FirstOrDefaultAsync(a => a.Email == emailOrUsername || a.Username == emailOrUsername);
    }

    public async Task<List<Account>> GetAllWithRolesAsync()
    {
        return await _context.Set<Account>()
            .Include(a => a.AccountRoles)
            .ThenInclude(ar => ar.Role)
            .ToListAsync();
    }

    public async Task<Account?> GetByIdWithRolesAsync(Guid id)
    {
        return await _context.Set<Account>()
            .Include(a => a.AccountRoles)
            .ThenInclude(ar => ar.Role)
            .FirstOrDefaultAsync(a => a.AccountId == id);
    }
}

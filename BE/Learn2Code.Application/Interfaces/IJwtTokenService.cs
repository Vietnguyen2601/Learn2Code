using Learn2Code.Domain.Entities;

namespace Learn2Code.Application.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(Account account, IList<string> roles);
}

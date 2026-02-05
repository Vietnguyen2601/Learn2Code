using Learn2Code.Application.DTOs;
using Learn2Code.Domain.Entities;

namespace Learn2Code.Application.Mapper;

public static class AccountMapper
{
    public static Account ToAccount(this RegisterRequest request, string password)
    {
        return new Account
        {
            AccountId = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            Password = password,
            Name = request.Name,
            PhoneNumber = request.PhoneNumber,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static RegisterResponse ToRegisterResponse(this Account account)
    {
        return new RegisterResponse
        {
            AccountId = account.AccountId,
            Username = account.Username,
            Email = account.Email,
            Name = account.Name
        };
    }

    public static LoginResponse ToLoginResponse(this Account account, string token, List<string> roles)
    {
        return new LoginResponse
        {
            AccountId = account.AccountId,
            Username = account.Username,
            Email = account.Email,
            Name = account.Name,
            Token = token,
            Roles = roles
        };
    }
    public static AccountDto ToAccountDto(this Account account, List<string> roles)
    {
        return new AccountDto
        {
            AccountId = account.AccountId,
            Username = account.Username,
            Email = account.Email,
            Name = account.Name,
            PhoneNumber = account.PhoneNumber,
            IsActive = account.IsActive,
            CreatedAt = account.CreatedAt ?? DateTime.MinValue,
            Roles = roles
        };
    }

    public static Account ToAccount(this CreateAccountRequest request, string password)
    {
        return new Account
        {
            AccountId = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            Password = password,
            Name = request.Name,
            PhoneNumber = request.PhoneNumber,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateAccount(this Account account, UpdateAccountRequest request)
    {
        if (request.Name != null) account.Name = request.Name;
        if (request.PhoneNumber != null) account.PhoneNumber = request.PhoneNumber;
        if (request.IsActive.HasValue) account.IsActive = request.IsActive.Value;
        account.UpdatedAt = DateTime.UtcNow;
    }
}

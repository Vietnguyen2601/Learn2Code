using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;
using Learn2Code.Application.Interfaces;
using Learn2Code.Application.Mapper;
using Learn2Code.Domain.Entities;
using Learn2Code.Infrastructure.Persistence.UnitOfWork;

namespace Learn2Code.Application.Services;

public class AccountService : IAccountService
{
    private readonly IUnitOfWork _unitOfWork;

    public AccountService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult<List<AccountDto>>> GetAllAccountsAsync()
    {
        var accounts = await _unitOfWork.AccountRepository.GetAllWithRolesAsync();

        var accountDtos = accounts.Select(a => a.ToAccountDto(
            a.AccountRoles?.Select(ar => ar.Role?.RoleName ?? "").ToList() ?? new List<string>()
        )).ToList();

        return ServiceResult<List<AccountDto>>.Ok(accountDtos);
    }

    public async Task<ServiceResult<AccountDto>> GetAccountByIdAsync(Guid id)
    {
        var account = await _unitOfWork.AccountRepository.GetByIdWithRolesAsync(id);
        if (account == null)
            return ServiceResult<AccountDto>.NotFound("Account not found");

        var roles = account.AccountRoles?.Select(ar => ar.Role?.RoleName ?? "").ToList() ?? new List<string>();

        return ServiceResult<AccountDto>.Ok(account.ToAccountDto(roles));
    }

    public async Task<ServiceResult<AccountDto>> CreateAccountAsync(CreateAccountRequest request)
    {
        var existingEmail = await _unitOfWork.AccountRepository.AnyAsync(a => a.Email == request.Email);
        if (existingEmail) return ServiceResult<AccountDto>.Error("EMAIL_EXISTS", "Email already exists");

        var existingUser = await _unitOfWork.AccountRepository.AnyAsync(a => a.Username == request.Username);
        if (existingUser) return ServiceResult<AccountDto>.Error("USERNAME_EXISTS", "Username already exists");

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var account = request.ToAccount(hashedPassword);

        _unitOfWork.AccountRepository.PrepareCreate(account);

        // Assign default Student role if no roles specified
        var rolesToAssign = request.Roles?.Count > 0 ? request.Roles : new List<string> { "Student" };

        foreach (var roleName in rolesToAssign)
        {
            var role = await _unitOfWork.RoleRepository.GetAsync(r => r.RoleName == roleName);
            if (role != null)
            {
                var accountRole = new AccountRole
                {
                    AccountId = account.AccountId,
                    RoleId = role.RoleId,
                    AssignedAt = DateTime.UtcNow
                };
                _unitOfWork.Repository<AccountRole>().PrepareCreate(accountRole);
            }
        }

        await _unitOfWork.CommitTransactionAsync();

        return ServiceResult<AccountDto>.Created(account.ToAccountDto(rolesToAssign));
    }

    public async Task<ServiceResult<AccountDto>> UpdateAccountAsync(Guid id, UpdateAccountRequest request)
    {
        var account = await _unitOfWork.AccountRepository.GetByIdWithRolesAsync(id);
        if (account == null) return ServiceResult<AccountDto>.NotFound("Account not found");

        account.UpdateAccount(request);

        _unitOfWork.AccountRepository.PrepareUpdate(account);
        await _unitOfWork.CommitTransactionAsync();

        var roles = account.AccountRoles?.Select(ar => ar.Role?.RoleName ?? "").ToList() ?? new List<string>();
        return ServiceResult<AccountDto>.Ok(account.ToAccountDto(roles));
    }

    public async Task<ServiceResult> DeleteAccountAsync(Guid id)
    {
        var account = await _unitOfWork.AccountRepository.GetAsync(a => a.AccountId == id);
        if (account == null) return ServiceResult.NotFound("Account not found");

        _unitOfWork.AccountRepository.PrepareRemove(account);
        await _unitOfWork.CommitTransactionAsync();

        return ServiceResult.Ok("Account deleted successfully");
    }
}


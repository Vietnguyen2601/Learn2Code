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
        var accounts = _unitOfWork.AccountRepository.GetAllQueryable().ToList();

        var accountDtos = accounts.Select(a => a.ToAccountDto(new List<string>())).ToList();

        return ServiceResult<List<AccountDto>>.Ok(accountDtos);
    }

    public async Task<ServiceResult<AccountDto>> GetAccountByIdAsync(Guid id)
    {
        var account = await _unitOfWork.AccountRepository.GetAsync(a => a.AccountId == id);
        if (account == null)
            return ServiceResult<AccountDto>.NotFound("Account not found");
        var roles = new List<string>();

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
        await _unitOfWork.CommitTransactionAsync();

        return ServiceResult<AccountDto>.Created(account.ToAccountDto(new List<string>()));
    }

    public async Task<ServiceResult<AccountDto>> UpdateAccountAsync(Guid id, UpdateAccountRequest request)
    {
        var account = await _unitOfWork.AccountRepository.GetAsync(a => a.AccountId == id);
        if (account == null) return ServiceResult<AccountDto>.NotFound("Account not found");

        account.UpdateAccount(request);

        _unitOfWork.AccountRepository.PrepareUpdate(account);
        await _unitOfWork.CommitTransactionAsync();

        return ServiceResult<AccountDto>.Ok(account.ToAccountDto(new List<string>()));
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

using Learn2Code.Application.DTOs;
using Learn2Code.Application.Base;

namespace Learn2Code.Application.Interfaces;

public interface IAccountService
{
    Task<ServiceResult<List<AccountDto>>> GetAllAccountsAsync();
    Task<ServiceResult<AccountDto>> GetAccountByIdAsync(Guid id);
    Task<ServiceResult<AccountDto>> CreateAccountAsync(CreateAccountRequest request);
    Task<ServiceResult<AccountDto>> UpdateAccountAsync(Guid id, UpdateAccountRequest request);
    Task<ServiceResult> DeleteAccountAsync(Guid id);
}

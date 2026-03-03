using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;

namespace Learn2Code.Application.Interfaces;

public interface IAuthService
{
    Task<ServiceResult> RegisterAsync(RegisterRequest request);
    Task<ServiceResult<VerifyOtpResponse>> VerifyOtpAsync(VerifyOtpRequest request);
    Task<ServiceResult<LoginResponse>> LoginAsync(LoginRequest request);
    Task<ServiceResult> ForgotPasswordAsync(ForgotPasswordRequest request);
    Task<ServiceResult<ResetPasswordResponse>> ResetPasswordAsync(ResetPasswordRequest request);
}

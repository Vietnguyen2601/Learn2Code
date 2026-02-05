using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;

namespace Learn2Code.Application.Interfaces;

public interface IAuthService
{
    Task<ServiceResult> SendOtpAsync(SendOtpRequest request);
    Task<ServiceResult<RegisterResponse>> RegisterAsync(RegisterRequest request);
    Task<ServiceResult<LoginResponse>> LoginAsync(LoginRequest request);
}

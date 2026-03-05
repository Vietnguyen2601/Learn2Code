using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;
using Learn2Code.Application.Interfaces;
using Learn2Code.Application.Mapper;
using Learn2Code.Application.Models;
using Learn2Code.Domain.Entities;
using Learn2Code.Infrastructure.Repositories.IRepository;
using Learn2Code.Infrastructure.Persistence.UnitOfWork;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Learn2Code.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<AuthService> _logger;
    private readonly IConfiguration _configuration;

    public AuthService(
        IUnitOfWork unitOfWork,
        IEmailService emailService,
        IJwtTokenService jwtTokenService,
        IMemoryCache memoryCache,
        ILogger<AuthService> logger,
        IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _jwtTokenService = jwtTokenService;
        _memoryCache = memoryCache;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<ServiceResult> RegisterAsync(RegisterRequest request)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Username) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return ServiceResult.Error("VALIDATION_ERROR", "Email, username and password are required");
        }

        if (request.Password.Length < 6)
        {
            return ServiceResult.Error("WEAK_PASSWORD", "Password must be at least 6 characters");
        }

        if (request.Password != request.ConfirmPassword)
        {
            return ServiceResult.Error("PASSWORD_MISMATCH", "Passwords do not match");
        }

        // Validate email format
        if (!IsValidEmail(request.Email))
        {
            return ServiceResult.Error("INVALID_EMAIL", "Invalid email format");
        }

        // Check if email already exists
        var existingEmail = await _unitOfWork.AccountRepository.GetAsync(a => a.Email == request.Email);
        if (existingEmail != null)
        {
            return ServiceResult.Error("EMAIL_EXISTS", "Email already registered");
        }

        // Check if username already exists
        var existingUsername = await _unitOfWork.AccountRepository.GetAsync(a => a.Username == request.Username);
        if (existingUsername != null)
        {
            return ServiceResult.Error("USERNAME_EXISTS", "Username already taken");
        }

        // Generate OTP
        var otpCode = GenerateOtpCode();

        // Store OTP and user registration data in memory cache
        var cacheKeyOtp = $"OTP_{request.Email}";
        var cacheKeyData = $"REGISTRATION_DATA_{request.Email}";
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

        _memoryCache.Set(cacheKeyOtp, otpCode, cacheOptions);
        _memoryCache.Set(cacheKeyData, request, cacheOptions);

        // Send OTP email
        var emailSent = await _emailService.SendOtpEmailAsync(request.Email, otpCode);
        if (!emailSent)
        {
            _memoryCache.Remove(cacheKeyOtp);
            _memoryCache.Remove(cacheKeyData);
            return ServiceResult.Error("EMAIL_SEND_FAILED", "Failed to send OTP email. Please try again.");
        }

        _logger.LogInformation("OTP sent to {Email}", request.Email);
        return ServiceResult.Ok("OTP sent successfully to your email. Please verify with the OTP code within 5 minutes.");
    }

    public async Task<ServiceResult<VerifyOtpResponse>> VerifyOtpAsync(VerifyOtpRequest request)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.OtpCode))
        {
            return ServiceResult<VerifyOtpResponse>.Error("VALIDATION_ERROR", "Email and OTP code are required");
        }

        // Validate email format
        if (!IsValidEmail(request.Email))
        {
            return ServiceResult<VerifyOtpResponse>.Error("INVALID_EMAIL", "Invalid email format");
        }

        // Verify OTP from memory cache
        var cacheKeyOtp = $"OTP_{request.Email}";
        if (!_memoryCache.TryGetValue(cacheKeyOtp, out string? cachedOtp) || cachedOtp != request.OtpCode)
        {
            return ServiceResult<VerifyOtpResponse>.Error("INVALID_OTP", "Invalid or expired OTP code");
        }

        // Get registration data from cache
        var cacheKeyData = $"REGISTRATION_DATA_{request.Email}";
        if (!_memoryCache.TryGetValue(cacheKeyData, out RegisterRequest? registrationData) || registrationData == null)
        {
            return ServiceResult<VerifyOtpResponse>.Error("SESSION_EXPIRED", "Registration session expired. Please register again.");
        }

        // Double check email and username existence
        var existingEmail = await _unitOfWork.AccountRepository.GetAsync(a => a.Email == request.Email);
        if (existingEmail != null)
        {
            return ServiceResult<VerifyOtpResponse>.Error("EMAIL_EXISTS", "Email already registered");
        }

        var existingUsername = await _unitOfWork.AccountRepository.GetAsync(a => a.Username == registrationData.Username);
        if (existingUsername != null)
        {
            return ServiceResult<VerifyOtpResponse>.Error("USERNAME_EXISTS", "Username already taken");
        }

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            // Remove OTP and registration data from cache after successful verification
            _memoryCache.Remove(cacheKeyOtp);
            _memoryCache.Remove(cacheKeyData);

            // Assign default role (Student)
            var studentRole = await _unitOfWork.RoleRepository.GetAsync(r => r.RoleName == "Student");
            if (studentRole == null)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ServiceResult<VerifyOtpResponse>.Error("ROLE_NOT_FOUND", "Default role 'Student' not found.");
            }

            // Create account
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registrationData.Password);
            var account = new Account
            {
                Email = registrationData.Email,
                Username = registrationData.Username,
                Password = hashedPassword,
                Name = registrationData.Name,
                PhoneNumber = registrationData.PhoneNumber,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _unitOfWork.AccountRepository.PrepareCreate(account);

            // Assign Student role
            var accountRole = new AccountRole
            {
                AccountId = account.AccountId,
                RoleId = studentRole.RoleId,
                AssignedAt = DateTime.UtcNow
            };
            _unitOfWork.Repository<AccountRole>().PrepareCreate(accountRole);

            await _unitOfWork.CommitTransactionAsync();

            var response = new VerifyOtpResponse
            {
                Username = account.Username,
                Email = account.Email,
                Name = account.Name
            };

            _logger.LogInformation("Account created successfully for {Email}", request.Email);
            return ServiceResult<VerifyOtpResponse>.Created(response, "Account created successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Failed to create account for {Email}", request.Email);
            return ServiceResult<VerifyOtpResponse>.Error("REGISTRATION_FAILED", "Failed to create account. Please try again.");
        }
    }

    public async Task<ServiceResult<LoginResponse>> LoginAsync(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.EmailOrUsername) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return ServiceResult<LoginResponse>.Error("VALIDATION_ERROR", "Email/username and password are required");
        }

        // Find account by email or username
        var account = await _unitOfWork.AccountRepository.GetByEmailOrUsernameWithRolesAsync(request.EmailOrUsername);

        if (account == null)
        {
            return ServiceResult<LoginResponse>.Error("INVALID_CREDENTIALS", "Invalid email/username or password");
        }

        if (!account.IsActive)
        {
            return ServiceResult<LoginResponse>.Error("ACCOUNT_INACTIVE", "Account is not active");
        }

        // Verify password
        if (!BCrypt.Net.BCrypt.Verify(request.Password, account.Password)) // Note: Property is Password or PasswordHash? Step 504 uses Password. Mapper uses PasswordHash. Conflict!
        {
            return ServiceResult<LoginResponse>.Error("INVALID_CREDENTIALS", "Invalid email/username or password");
        }

        // Get roles
        var roles = account.AccountRoles
            .Where(ar => ar.Role.IsActive)
            .Select(ar => ar.Role.RoleName)
            .ToList();

        // Generate JWT access token and refresh token
        var token = _jwtTokenService.GenerateToken(account, roles);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        var jwtSettings = _configuration.GetSection("JwtSettings");
        var refreshTokenExpirationDays = int.Parse(jwtSettings["RefreshTokenExpirationDays"] ?? "7");

        account.RefreshToken = refreshToken;
        account.RefreshTokenExpiry = DateTime.UtcNow.AddDays(refreshTokenExpirationDays);
        account.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.AccountRepository.PrepareUpdate(account);
        await _unitOfWork.SaveChangesAsync();

        var response = account.ToLoginResponse(token, refreshToken, roles);

        _logger.LogInformation("User {Username} logged in successfully", account.Username);
        return ServiceResult<LoginResponse>.Ok(response, "Login successful");
    }

    public async Task<ServiceResult> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return ServiceResult.Error("INVALID_EMAIL", "Email is required");
        }

        // Validate email format
        if (!IsValidEmail(request.Email))
        {
            return ServiceResult.Error("INVALID_EMAIL", "Invalid email format");
        }

        // Check if account exists
        var account = await _unitOfWork.AccountRepository.GetAsync(a => a.Email == request.Email);
        if (account == null)
        {
            return ServiceResult.Error("ACCOUNT_NOT_FOUND", "Account with this email does not exist");
        }

        // Generate OTP
        var otpCode = GenerateOtpCode();

        // Store OTP in memory cache with key for password reset
        var cacheKey = $"PASSWORD_RESET_OTP_{request.Email}";
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

        _memoryCache.Set(cacheKey, otpCode, cacheOptions);

        // Send OTP email
        var emailSent = await _emailService.SendOtpEmailAsync(request.Email, otpCode);
        if (!emailSent)
        {
            return ServiceResult.Error("EMAIL_SEND_FAILED", "Failed to send OTP email. Please try again.");
        }

        _logger.LogInformation("Password reset OTP sent to {Email}", request.Email);
        return ServiceResult.Ok("OTP sent successfully. Please check your email.");
    }

    public async Task<ServiceResult<ResetPasswordResponse>> ResetPasswordAsync(ResetPasswordRequest request)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.OtpCode) ||
            string.IsNullOrWhiteSpace(request.NewPassword))
        {
            return ServiceResult<ResetPasswordResponse>.Error("VALIDATION_ERROR", "Email, OTP code and new password are required");
        }

        if (request.NewPassword.Length < 6)
        {
            return ServiceResult<ResetPasswordResponse>.Error("WEAK_PASSWORD", "Password must be at least 6 characters");
        }

        if (request.NewPassword != request.ConfirmPassword)
        {
            return ServiceResult<ResetPasswordResponse>.Error("PASSWORD_MISMATCH", "Passwords do not match");
        }

        // Validate email format
        if (!IsValidEmail(request.Email))
        {
            return ServiceResult<ResetPasswordResponse>.Error("INVALID_EMAIL", "Invalid email format");
        }

        // Verify OTP from memory cache
        var cacheKey = $"PASSWORD_RESET_OTP_{request.Email}";
        if (!_memoryCache.TryGetValue(cacheKey, out string? cachedOtp) || cachedOtp != request.OtpCode)
        {
            return ServiceResult<ResetPasswordResponse>.Error("INVALID_OTP", "Invalid or expired OTP code");
        }

        // Check if account exists
        var account = await _unitOfWork.AccountRepository.GetAsync(a => a.Email == request.Email);
        if (account == null)
        {
            return ServiceResult<ResetPasswordResponse>.Error("ACCOUNT_NOT_FOUND", "Account with this email does not exist");
        }

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            // Remove OTP from cache after successful verification
            _memoryCache.Remove(cacheKey);

            // Update password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            account.Password = hashedPassword;

            _unitOfWork.AccountRepository.PrepareUpdate(account);
            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation("Password reset successfully for {Email}", request.Email);

            var response = new ResetPasswordResponse
            {
                Email = account.Email,
                Message = "Password reset successfully"
            };

            return ServiceResult<ResetPasswordResponse>.Ok(response, "Password reset successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Failed to reset password for {Email}", request.Email);
            return ServiceResult<ResetPasswordResponse>.Error("RESET_FAILED", "Failed to reset password. Please try again.");
        }
    }

    public async Task<ServiceResult<RefreshTokenResponse>> RefreshTokenAsync(RefreshTokenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.AccessToken) || string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return ServiceResult<RefreshTokenResponse>.Error("VALIDATION_ERROR", "Access token and refresh token are required");
        }

        var principal = _jwtTokenService.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal == null)
        {
            return ServiceResult<RefreshTokenResponse>.Error("INVALID_TOKEN", "Invalid access token");
        }

        // JwtSecurityTokenHandler maps "sub" → ClaimTypes.NameIdentifier on validation
        var accountIdStr = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!Guid.TryParse(accountIdStr, out var accountId))
        {
            return ServiceResult<RefreshTokenResponse>.Error("INVALID_TOKEN", "Invalid token claims");
        }

        var account = await _unitOfWork.AccountRepository.GetByIdWithRolesAsync(accountId);
        if (account == null)
        {
            return ServiceResult<RefreshTokenResponse>.Error("ACCOUNT_NOT_FOUND", "Account not found");
        }

        if (!account.IsActive)
        {
            return ServiceResult<RefreshTokenResponse>.Error("ACCOUNT_INACTIVE", "Account is not active");
        }

        if (account.RefreshToken != request.RefreshToken || account.RefreshTokenExpiry < DateTime.UtcNow)
        {
            return ServiceResult<RefreshTokenResponse>.Error("INVALID_REFRESH_TOKEN", "Invalid or expired refresh token");
        }

        var roles = account.AccountRoles
            .Where(ar => ar.Role.IsActive)
            .Select(ar => ar.Role.RoleName)
            .ToList();

        var jwtSettings = _configuration.GetSection("JwtSettings");
        var refreshTokenExpirationDays = int.Parse(jwtSettings["RefreshTokenExpirationDays"] ?? "7");

        var newAccessToken = _jwtTokenService.GenerateToken(account, roles);
        var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

        account.RefreshToken = newRefreshToken;
        account.RefreshTokenExpiry = DateTime.UtcNow.AddDays(refreshTokenExpirationDays);
        account.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.AccountRepository.PrepareUpdate(account);

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Token refreshed for account {AccountId}", accountId);
        return ServiceResult<RefreshTokenResponse>.Ok(new RefreshTokenResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        }, "Token refreshed successfully");
    }

    public async Task<ServiceResult<MeResponse>> GetMeAsync(Guid accountId)
    {
        var account = await _unitOfWork.AccountRepository.GetByIdWithRolesAsync(accountId);
        if (account == null)
            return ServiceResult<MeResponse>.Error("ACCOUNT_NOT_FOUND", "Account not found");

        var roles = account.AccountRoles
            .Where(ar => ar.Role.IsActive)
            .Select(ar => ar.Role.RoleName)
            .ToList();

        return ServiceResult<MeResponse>.Ok(new MeResponse
        {
            AccountId   = account.AccountId,
            Username    = account.Username,
            Email       = account.Email,
            Name        = account.Name,
            PhoneNumber = account.PhoneNumber,
            IsActive    = account.IsActive,
            Roles       = roles,
            CreatedAt   = account.CreatedAt
        });
    }

    public async Task<ServiceResult<UpdateProfileResponse>> UpdateProfileAsync(Guid accountId, UpdateProfileRequest request)
    {
        var account = await _unitOfWork.AccountRepository.GetByIdAsync(accountId);
        if (account == null)
            return ServiceResult<UpdateProfileResponse>.Error("ACCOUNT_NOT_FOUND", "Account not found");

        if (request.Name != null)
            account.Name = request.Name;

        if (request.PhoneNumber != null)
            account.PhoneNumber = request.PhoneNumber;

        account.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.AccountRepository.PrepareUpdate(account);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Profile updated for account {AccountId}", accountId);
        return ServiceResult<UpdateProfileResponse>.Ok(new UpdateProfileResponse
        {
            Username    = account.Username,
            Email       = account.Email,
            Name        = account.Name,
            PhoneNumber = account.PhoneNumber
        }, "Profile updated successfully");
    }

    public async Task<ServiceResult> LogoutAsync(Guid accountId)
    {
        var account = await _unitOfWork.AccountRepository.GetByIdAsync(accountId);
        if (account == null)
            return ServiceResult.Error("ACCOUNT_NOT_FOUND", "Account not found");

        account.RefreshToken       = null;
        account.RefreshTokenExpiry = null;
        account.UpdatedAt          = DateTime.UtcNow;
        _unitOfWork.AccountRepository.PrepareUpdate(account);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Account {AccountId} logged out", accountId);
        return ServiceResult.Ok("Logged out successfully");
    }

    private static string GenerateOtpCode()
    {
        var random = new Random();
        return random.Next(100000, 999999).ToString();
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}

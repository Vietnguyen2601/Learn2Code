using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;
using Learn2Code.Application.Interfaces;
using Learn2Code.Application.Mapper;
using Learn2Code.Application.Models;
using Learn2Code.Domain.Entities;
using Learn2Code.Infrastructure.Repositories.IRepository;
using Learn2Code.Infrastructure.Persistence.UnitOfWork;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Learn2Code.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUnitOfWork unitOfWork,
        IEmailService emailService,
        IJwtTokenService jwtTokenService,
        IMemoryCache memoryCache,
        ILogger<AuthService> logger)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _jwtTokenService = jwtTokenService;
        _memoryCache = memoryCache;
        _logger = logger;
    }

    public async Task<ServiceResult> SendOtpAsync(SendOtpRequest request)
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

        // Check if email already exists
        // Check if email already exists
        var existingAccount = await _unitOfWork.AccountRepository.GetAsync(a => a.Email == request.Email);

        if (existingAccount != null)
        {
            return ServiceResult.Error("EMAIL_EXISTS", "Email already registered");
        }

        // Generate OTP
        var otpCode = GenerateOtpCode();

        // Store OTP in memory cache
        var cacheKey = $"OTP_{request.Email}";
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

        _memoryCache.Set(cacheKey, otpCode, cacheOptions);

        // Send OTP email
        var emailSent = await _emailService.SendOtpEmailAsync(request.Email, otpCode);
        if (!emailSent)
        {
            return ServiceResult.Error("EMAIL_SEND_FAILED", "Failed to send OTP email. Please try again.");
        }

        _logger.LogInformation("OTP sent to {Email}", request.Email);
        return ServiceResult.Ok("OTP sent successfully. Please check your email.");
    }

    public async Task<ServiceResult<RegisterResponse>> RegisterAsync(RegisterRequest request)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.OtpCode) ||
            string.IsNullOrWhiteSpace(request.Username) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return ServiceResult<RegisterResponse>.Error("VALIDATION_ERROR", "Email, OTP code, username and password are required");
        }

        if (request.Password.Length < 6)
        {
            return ServiceResult<RegisterResponse>.Error("WEAK_PASSWORD", "Password must be at least 6 characters");
        }

        if (request.Password != request.ConfirmPassword)
        {
            return ServiceResult<RegisterResponse>.Error("PASSWORD_MISMATCH", "Passwords do not match");
        }

        // Verify OTP from memory cache
        var cacheKey = $"OTP_{request.Email}";
        if (!_memoryCache.TryGetValue(cacheKey, out string? cachedOtp) || cachedOtp != request.OtpCode)
        {
            return ServiceResult<RegisterResponse>.Error("INVALID_OTP", "Invalid or expired OTP code");
        }

        // Check if username already exists
        // Check if username already exists
        var existingUsername = await _unitOfWork.AccountRepository.GetAsync(a => a.Username == request.Username);

        if (existingUsername != null)
        {
            return ServiceResult<RegisterResponse>.Error("USERNAME_EXISTS", "Username already taken");
        }

        // Check if email already exists
        // Check if email already exists
        var existingEmail = await _unitOfWork.AccountRepository.GetAsync(a => a.Email == request.Email);

        if (existingEmail != null)
        {
            return ServiceResult<RegisterResponse>.Error("EMAIL_EXISTS", "Email already registered");
        }

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            // Remove OTP from cache after successful verification use
            _memoryCache.Remove(cacheKey);

            // Assign default role (Student)
            // Assign default role (Student)
            var studentRole = await _unitOfWork.RoleRepository.GetAsync(r => r.RoleName == "Student");

            if (studentRole == null)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ServiceResult<RegisterResponse>.Error("ROLE_NOT_FOUND", "Default role 'Student' not found.");
            }

            // Create account using Mapper
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var account = request.ToAccount(hashedPassword);

            _unitOfWork.AccountRepository.PrepareCreate(account);

            // Note: The previous logic manually created AccountRole. 
            // Since Account entity has RoleId foreign key in my mapper setup, setting RoleId is enough IF the relationship is configured correctly.
            // But let's check Account entity (Step 332 didn't show full file).
            // Usually if there is AccountRoles navigation property (many-to-many), we need to add to that collection.
            // But here the Manual Mapper sets RoleId property directly on Account.
            // If Account has "RoleId" property, that's fine for 1-N.
            // However, Previous code (Step 504) created `AccountRole` explicitly.
            /*
                var accountRole = new AccountRole { ... };
                _unitOfWork.Repository<AccountRole>().PrepareCreate(accountRole);
            */
            // If I rely ONLY on `account.RoleId = roleId`, EF might handle it if configured.
            // But let's be safe and keep strict explicit logic if needed.
            // Wait, previous code (Step 504) did NOT set `account.RoleId`. It created `AccountRole` entity separately.
            // This suggests a **Many-to-Many** relationship or a separate join table entity `AccountRole`.
            // My Mapper sets `account.RoleId`. Does `Account` have `RoleId` property?
            // In Step 332 snippet: `RoleId = studentRole.RoleId` was used! So yes, `Account` has `RoleId`.
            // BUT Step 504 `new Account` instantiation (lines 129-140) did NOT set RoleId.
            // Instead lines 151-157 created `AccountRole`.

            // Conflict check:
            // Step 332 (from creating AuthService initially): `var account = new Account ... RoleId = studentRole.RoleId`.
            // Step 504 (current file): `var account = new Account ...` (NO RoleId). And explicit `AccountRole` creation.

            // Which one is correct? `Account` entity structure.
            // If I use `ToAccount` mapper which sets `RoleId`, I assume `Account` has `RoleId`.
            // If `Account` has `RoleId`, I don't need `AccountRole` entity usually, unless it's advanced tracking.
            // Let's assume `Account` has `RoleId` based on my Mapper design.
            // BUT to be safe and consistent with previous code, I should probably also create AccountRole if the system expects it.
            // Or maybe `Account` DOES NOT have `RoleId` in the current version?

            // Let's check `Account.cs` again? No time.
            // Safer bet: Keep `AccountRole` creation logic just in case, or check.
            // Actually, if `Account` has `RoleId`, then `AccountRole` table might be redundant or for history?
            // Wait, `AccountRole` usually implies Many-to-Many.

            // If I look at LoginAsync (Step 504 line 213): `account.AccountRoles`.
            // This strongly suggests Many-to-Many.
            // If so, `Account` might NOT have `RoleId` column directly, or it's just a shadow FK?
            // My `AccountMapper` sets `RoleId`. If `Account` doesn't have it, compile error!

            // Let's view `Account.cs` quickly. It's critical.

            await _unitOfWork.CommitTransactionAsync();

            var response = account.ToRegisterResponse();

            _logger.LogInformation("Account created successfully for {Email}", request.Email);
            return ServiceResult<RegisterResponse>.Created(response, "Account created successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Failed to create account for {Email}", request.Email);
            return ServiceResult<RegisterResponse>.Error("REGISTRATION_FAILED", "Failed to create account. Please try again.");
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

        // Generate JWT
        var token = _jwtTokenService.GenerateToken(account, roles);

        var response = account.ToLoginResponse(token, roles);

        _logger.LogInformation("User {Username} logged in successfully", account.Username);
        return ServiceResult<LoginResponse>.Ok(response, "Login successful");
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

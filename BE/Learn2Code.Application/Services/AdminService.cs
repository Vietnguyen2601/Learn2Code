using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs.AdminDTOs.AdminRequests;
using Learn2Code.Application.DTOs.AdminDTOs.AdminResponses;
using Learn2Code.Application.Interfaces;
using Learn2Code.Domain.Entities;
using Learn2Code.Domain.Enums;
using Learn2Code.Infrastructure.Persistence.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Learn2Code.Application.Services;

public class AdminService : IAdminService
{
    private readonly IUnitOfWork _unitOfWork;

    public AdminService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // ─── User Management ───────────────────────────

    public async Task<ServiceResult<List<AdminUserDto>>> GetAllUsersAsync(string? search, string? role, bool? isActive)
    {
        var query = _unitOfWork.AccountRepository
            .GetAllQueryable()
            .Include(a => a.AccountRoles)
                .ThenInclude(ar => ar.Role)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.ToLower();
            query = query.Where(a =>
                a.Username.ToLower().Contains(s) ||
                a.Email.ToLower().Contains(s) ||
                (a.Name != null && a.Name.ToLower().Contains(s)));
        }

        if (isActive.HasValue)
            query = query.Where(a => a.IsActive == isActive.Value);

        if (!string.IsNullOrWhiteSpace(role))
            query = query.Where(a => a.AccountRoles.Any(ar => ar.Role.RoleName == role));

        var accounts = await query.OrderBy(a => a.Username).ToListAsync();

        var dtos = accounts.Select(a => new AdminUserDto
        {
            AccountId = a.AccountId,
            Username = a.Username,
            Email = a.Email,
            Name = a.Name,
            PhoneNumber = a.PhoneNumber,
            IsActive = a.IsActive,
            Roles = a.AccountRoles.Select(ar => ar.Role.RoleName).ToList(),
            CreatedAt = a.CreatedAt
        }).ToList();

        return ServiceResult<List<AdminUserDto>>.Ok(dtos);
    }

    public async Task<ServiceResult<AdminUserDto>> GetUserByIdAsync(Guid userId)
    {
        var account = await _unitOfWork.AccountRepository
            .GetAllQueryable()
            .Include(a => a.AccountRoles)
                .ThenInclude(ar => ar.Role)
            .FirstOrDefaultAsync(a => a.AccountId == userId);

        if (account == null)
            return ServiceResult<AdminUserDto>.NotFound("User not found");

        var dto = new AdminUserDto
        {
            AccountId = account.AccountId,
            Username = account.Username,
            Email = account.Email,
            Name = account.Name,
            PhoneNumber = account.PhoneNumber,
            IsActive = account.IsActive,
            Roles = account.AccountRoles.Select(ar => ar.Role.RoleName).ToList(),
            CreatedAt = account.CreatedAt
        };

        return ServiceResult<AdminUserDto>.Ok(dto);
    }

    public async Task<ServiceResult<AdminUserDto>> UpdateUserAsync(Guid userId, UpdateUserRequest request)
    {
        var account = await _unitOfWork.AccountRepository
            .GetAllQueryable()
            .Include(a => a.AccountRoles)
                .ThenInclude(ar => ar.Role)
            .FirstOrDefaultAsync(a => a.AccountId == userId);

        if (account == null)
            return ServiceResult<AdminUserDto>.NotFound("User not found");

        // Update is_active
        if (request.IsActive.HasValue)
        {
            account.IsActive = request.IsActive.Value;
            account.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.AccountRepository.PrepareUpdate(account);
        }

        // Update role (replace all existing roles with new one)
        if (!string.IsNullOrWhiteSpace(request.Role))
        {
            var newRole = await _unitOfWork.RoleRepository
                .GetAllQueryable()
                .FirstOrDefaultAsync(r => r.RoleName == request.Role);

            if (newRole == null)
                return ServiceResult<AdminUserDto>.Error("INVALID_ROLE",
                    $"Role '{request.Role}' does not exist");

            // Remove existing roles
            foreach (var ar in account.AccountRoles.ToList())
            {
                _unitOfWork.Repository<AccountRole>().PrepareRemove(ar);
            }

            // Add new role
            _unitOfWork.Repository<AccountRole>().PrepareCreate(new AccountRole
            {
                AccountId = account.AccountId,
                RoleId = newRole.RoleId,
                AssignedAt = DateTime.UtcNow
            });
        }

        await _unitOfWork.SaveChangesAsync();

        // Reload to get updated data
        return await GetUserByIdAsync(userId);
    }

    // ─── Dashboard ─────────────────────────────────

    public async Task<ServiceResult<DashboardDto>> GetDashboardAsync()
    {
        var totalUsers = await _unitOfWork.AccountRepository.GetAllQueryable().CountAsync();
        var totalCourses = await _unitOfWork.CourseRepository.GetAllQueryable().CountAsync();
        var totalEnrollments = await _unitOfWork.EnrollmentRepository.GetAllQueryable().CountAsync();

        var totalRevenue = await _unitOfWork.PaymentRepository
            .GetAllQueryable()
            .Where(p => p.Status == PaymentStatus.Success)
            .SumAsync(p => p.Amount);

        var activeSubscriptions = await _unitOfWork.SubscriptionRepository
            .GetAllQueryable()
            .Where(s => s.Status == SubscriptionStatus.Active)
            .CountAsync();

        var dto = new DashboardDto
        {
            TotalUsers = totalUsers,
            TotalCourses = totalCourses,
            TotalEnrollments = totalEnrollments,
            TotalRevenue = totalRevenue,
            ActiveSubscriptions = activeSubscriptions
        };

        return ServiceResult<DashboardDto>.Ok(dto);
    }

    public async Task<ServiceResult<List<RevenueByMonthDto>>> GetRevenueByMonthAsync(int months = 12)
    {
        var since = DateTime.UtcNow.AddMonths(-months);

        var payments = await _unitOfWork.PaymentRepository
            .GetAllQueryable()
            .Where(p => p.Status == PaymentStatus.Success && p.PaidAt != null && p.PaidAt >= since)
            .ToListAsync();

        var grouped = payments
            .GroupBy(p => new { p.PaidAt!.Value.Year, p.PaidAt!.Value.Month })
            .Select(g => new RevenueByMonthDto
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Revenue = g.Sum(p => p.Amount),
                PaymentCount = g.Count()
            })
            .OrderBy(r => r.Year).ThenBy(r => r.Month)
            .ToList();

        return ServiceResult<List<RevenueByMonthDto>>.Ok(grouped);
    }

    public async Task<ServiceResult<List<EnrollmentByCourseDashboardDto>>> GetEnrollmentsByCourseDashboardAsync()
    {
        var courses = await _unitOfWork.CourseRepository.GetAllQueryable().ToListAsync();
        var enrollments = await _unitOfWork.EnrollmentRepository.GetAllQueryable().ToListAsync();

        var dtos = courses.Select(c => new EnrollmentByCourseDashboardDto
        {
            CourseId = c.CourseId,
            CourseTitle = c.Title,
            EnrollmentCount = enrollments.Count(e => e.CourseId == c.CourseId),
            CompletedCount = enrollments.Count(e => e.CourseId == c.CourseId
                                                    && e.Status == EnrollmentStatus.Completed)
        })
        .OrderByDescending(d => d.EnrollmentCount)
        .ToList();

        return ServiceResult<List<EnrollmentByCourseDashboardDto>>.Ok(dtos);
    }
}

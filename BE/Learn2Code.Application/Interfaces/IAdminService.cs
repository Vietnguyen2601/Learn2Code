using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs.AdminDTOs.AdminRequests;
using Learn2Code.Application.DTOs.AdminDTOs.AdminResponses;

namespace Learn2Code.Application.Interfaces;

public interface IAdminService
{
    // Users
    Task<ServiceResult<List<AdminUserDto>>> GetAllUsersAsync(string? search, string? role, bool? isActive);
    Task<ServiceResult<AdminUserDto>> GetUserByIdAsync(Guid userId);
    Task<ServiceResult<AdminUserDto>> UpdateUserAsync(Guid userId, UpdateUserRequest request);

    // Dashboard
    Task<ServiceResult<DashboardDto>> GetDashboardAsync();
    Task<ServiceResult<List<RevenueByMonthDto>>> GetRevenueByMonthAsync(int months = 12);
    Task<ServiceResult<List<EnrollmentByCourseDashboardDto>>> GetEnrollmentsByCourseDashboardAsync();
}

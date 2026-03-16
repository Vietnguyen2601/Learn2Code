using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;
using Learn2Code.Domain.Enums;

namespace Learn2Code.Application.Interfaces;

public interface ICourseService
{
    Task<ServiceResult<List<CourseDto>>> GetAllCoursesAsync(
        Guid? categoryId = null,
        CourseDifficulty? difficulty = null,
        string? search = null);
    
    Task<ServiceResult<List<CourseDto>>> GetActiveCoursesAsync(
        Guid? categoryId = null,
        CourseDifficulty? difficulty = null,
        string? search = null);
    
    Task<ServiceResult<List<CourseDto>>> GetInactiveCoursesAsync(
        Guid? categoryId = null,
        CourseDifficulty? difficulty = null,
        string? search = null);
    
    Task<ServiceResult<CourseDto>> GetCourseByIdAsync(Guid id);
    Task<ServiceResult<CourseDetailDto>> GetCourseDetailByIdAsync(Guid id);
    Task<ServiceResult<CourseDto>> CreateCourseAsync(CreateCourseRequest request);
    Task<ServiceResult<CourseDto>> UpdateCourseAsync(Guid id, UpdateCourseRequest request);
    Task<ServiceResult> DeleteCourseAsync(Guid id);
    Task<ServiceResult<CourseDto>> RestoreCourseAsync(Guid id);
}

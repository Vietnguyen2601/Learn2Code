using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;

namespace Learn2Code.Application.Interfaces;

public interface IProgressService
{
    Task<ServiceResult<CourseProgressDto>> GetMyCourseProgressAsync(Guid studentId, Guid courseId);
    Task<ServiceResult<LessonProgressDetailDto>> GetMyLessonProgressAsync(Guid studentId, Guid lessonId);
    Task<ServiceResult<AllStudentsProgressDto>> GetAllStudentsProgressAsync(Guid courseId);
}

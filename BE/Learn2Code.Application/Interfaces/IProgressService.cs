using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;

namespace Learn2Code.Application.Interfaces;

public interface IProgressService
{
    Task<ServiceResult<CourseProgressDto>> GetCourseProgressAsync(Guid courseId, Guid studentId);
    Task<ServiceResult<LessonProgressDetailDto>> GetLessonProgressAsync(Guid lessonId, Guid studentId);
    Task<ServiceResult<LessonProgressDto>> UpdateLessonProgressAsync(Guid lessonId, Guid studentId, UpdateLessonProgressRequest request);
}

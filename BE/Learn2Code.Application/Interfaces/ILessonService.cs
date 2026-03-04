using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;

namespace Learn2Code.Application.Interfaces;

public interface ILessonService
{
    /// <summary>
    /// Get lesson detail with access control checks
    /// </summary>
    Task<ServiceResult<LessonDetailDto>> GetLessonAsync(Guid lessonId, Guid studentId);
}

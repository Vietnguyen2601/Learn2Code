using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;

namespace Learn2Code.Application.Interfaces;

public interface ILessonService
{
    Task<ServiceResult<List<LessonDto>>> GetLessonsBySectionIdAsync(Guid sectionId);
    Task<ServiceResult<LessonDetailDto>> GetLessonByIdAsync(Guid lessonId, Guid? userId);
    Task<ServiceResult<LessonDto>> CreateLessonAsync(Guid sectionId, CreateLessonRequest request);
    Task<ServiceResult<LessonDto>> UpdateLessonAsync(Guid lessonId, UpdateLessonRequest request);
    Task<ServiceResult> DeleteLessonAsync(Guid lessonId);
    Task<ServiceResult> ReorderLessonsAsync(Guid sectionId, ReorderLessonsRequest request);
}

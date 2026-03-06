using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;

namespace Learn2Code.Application.Interfaces;

public interface ISectionService
{
    Task<ServiceResult<List<SectionDto>>> GetSectionsByCourseIdAsync(Guid courseId);
    Task<ServiceResult<SectionDto>> GetSectionByIdAsync(Guid sectionId);
    Task<ServiceResult<SectionDto>> CreateSectionAsync(Guid courseId, CreateSectionRequest request);
    Task<ServiceResult<SectionDto>> UpdateSectionAsync(Guid courseId, Guid sectionId, UpdateSectionRequest request);
    Task<ServiceResult> DeleteSectionAsync(Guid courseId, Guid sectionId);
    Task<ServiceResult<List<SectionDto>>> ReorderSectionsAsync(Guid courseId, ReorderSectionsRequest request);
}

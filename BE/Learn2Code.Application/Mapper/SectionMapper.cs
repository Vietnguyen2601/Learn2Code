using Learn2Code.Application.DTOs;
using Learn2Code.Domain.Entities;

namespace Learn2Code.Application.Mapper;

public static class SectionMapper
{
    public static SectionDto ToDto(this Section section)
    {
        return new SectionDto
        {
            SectionId = section.SectionId,
            CourseId = section.CourseId,
            Title = section.Title,
            Description = section.Description,
            OrderNumber = section.OrderNumber,
            IsActive = section.IsActive,
            CreatedAt = section.CreatedAt,
            UpdatedAt = section.UpdatedAt
        };
    }

    public static Section ToEntity(this CreateSectionRequest request)
    {
        return new Section
        {
            SectionId = Guid.NewGuid(),
            CourseId = request.CourseId,
            Title = request.Title,
            Description = request.Description,
            OrderNumber = request.OrderNumber,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateSection(this Section section, UpdateSectionRequest request)
    {
        if (request.Title != null)
            section.Title = request.Title;

        if (request.Description != null)
            section.Description = request.Description;

        if (request.OrderNumber.HasValue)
            section.OrderNumber = request.OrderNumber.Value;

        if (request.IsActive.HasValue)
            section.IsActive = request.IsActive.Value;

        section.UpdatedAt = DateTime.UtcNow;
    }
}

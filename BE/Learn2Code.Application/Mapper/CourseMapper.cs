using Learn2Code.Application.DTOs;
using Learn2Code.Application.DTOs.CourseDTOs.CourseRequests;
using Learn2Code.Application.DTOs.CourseDTOs.CourseResponses;
using Learn2Code.Application.DTOs.SectionDTOs.SectionResponses;
using Learn2Code.Domain.Entities;
using Learn2Code.Domain.Enums;

namespace Learn2Code.Application.Mapper;

public static class CourseMapper
{
    public static CourseDto ToDto(this Course course)
    {
        return new CourseDto
        {
            CourseId = course.CourseId,
            Title = course.Title,
            Description = course.Description,
            Difficulty = course.Difficulty?.ToString(),
            IsActive = course.IsActive,
            CategoryId = course.CategoryId,
            CategoryName = course.Category?.Name,
            CreatedAt = course.CreatedAt,
            UpdatedAt = course.UpdatedAt
        };
    }

    public static CourseDetailDto ToDetailDto(this Course course)
    {
        return new CourseDetailDto
        {
            CourseId = course.CourseId,
            Title = course.Title,
            Description = course.Description,
            Difficulty = course.Difficulty?.ToString(),
            IsActive = course.IsActive,
            CategoryId = course.CategoryId,
            CategoryName = course.Category?.Name,
            CreatedAt = course.CreatedAt,
            UpdatedAt = course.UpdatedAt,
            Sections = course.Sections?.Select(s => s.ToDto()).ToList() ?? new List<SectionDto>()
        };
    }

    public static Course ToEntity(this CreateCourseRequest request)
    {
        CourseDifficulty? difficulty = null;
        if (!string.IsNullOrWhiteSpace(request.Difficulty) && 
            Enum.TryParse<CourseDifficulty>(request.Difficulty, true, out var parsedDifficulty))
        {
            difficulty = parsedDifficulty;
        }

        return new Course
        {
            CourseId = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Difficulty = difficulty,
            CategoryId = request.CategoryId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateCourse(this Course course, UpdateCourseRequest request)
    {
        if (request.Title != null) 
            course.Title = request.Title;
        
        if (request.Description != null) 
            course.Description = request.Description;
        
        if (request.Difficulty != null && 
            Enum.TryParse<CourseDifficulty>(request.Difficulty, true, out var parsedDifficulty))
        {
            course.Difficulty = parsedDifficulty;
        }
        
        if (request.CategoryId.HasValue) 
            course.CategoryId = request.CategoryId.Value;
        
        if (request.IsActive.HasValue) 
            course.IsActive = request.IsActive.Value;
        
        course.UpdatedAt = DateTime.UtcNow;
    }
}

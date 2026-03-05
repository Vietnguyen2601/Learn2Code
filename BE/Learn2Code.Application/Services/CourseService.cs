using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;
using Learn2Code.Application.Interfaces;
using Learn2Code.Application.Mapper;
using Learn2Code.Domain.Enums;
using Learn2Code.Infrastructure.Persistence.UnitOfWork;

namespace Learn2Code.Application.Services;

public class CourseService : ICourseService
{
    private readonly IUnitOfWork _unitOfWork;

    public CourseService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult<List<CourseDto>>> GetAllCoursesAsync(
        Guid? categoryId = null,
        CourseDifficulty? difficulty = null,
        string? search = null)
    {
        var courses = await _unitOfWork.CourseRepository.GetAllWithRelationsAsync(
            categoryId, 
            difficulty, 
            search);

        var courseDtos = courses.Select(c => c.ToDto()).ToList();

        return ServiceResult<List<CourseDto>>.Ok(courseDtos);
    }

    public async Task<ServiceResult<List<CourseDto>>> GetActiveCoursesAsync(
        Guid? categoryId = null,
        CourseDifficulty? difficulty = null,
        string? search = null)
    {
        var courses = await _unitOfWork.CourseRepository.GetAllWithRelationsByStatusAsync(
            true,
            categoryId,
            difficulty,
            search);

        var courseDtos = courses.Select(c => c.ToDto()).ToList();

        return ServiceResult<List<CourseDto>>.Ok(courseDtos);
    }

    public async Task<ServiceResult<List<CourseDto>>> GetInactiveCoursesAsync(
        Guid? categoryId = null,
        CourseDifficulty? difficulty = null,
        string? search = null)
    {
        var courses = await _unitOfWork.CourseRepository.GetAllWithRelationsByStatusAsync(
            false,
            categoryId,
            difficulty,
            search);

        var courseDtos = courses.Select(c => c.ToDto()).ToList();

        return ServiceResult<List<CourseDto>>.Ok(courseDtos);
    }

    public async Task<ServiceResult<CourseDto>> GetCourseByIdAsync(Guid id)
    {
        var course = await _unitOfWork.CourseRepository.GetByIdWithRelationsAsync(id);
        if (course == null)
            return ServiceResult<CourseDto>.NotFound("Course not found");

        return ServiceResult<CourseDto>.Ok(course.ToDto());
    }

    public async Task<ServiceResult<CourseDetailDto>> GetCourseDetailByIdAsync(Guid id)
    {
        var course = await _unitOfWork.CourseRepository.GetByIdWithRelationsAsync(id);
        if (course == null)
            return ServiceResult<CourseDetailDto>.NotFound("Course not found");

        return ServiceResult<CourseDetailDto>.Ok(course.ToDetailDto());
    }

    public async Task<ServiceResult<CourseDto>> CreateCourseAsync(CreateCourseRequest request)
    {
        // Validate difficulty enum if provided
        if (!string.IsNullOrWhiteSpace(request.Difficulty))
        {
            if (!Enum.TryParse<CourseDifficulty>(request.Difficulty, true, out _))
            {
                return ServiceResult<CourseDto>.Error(
                    "INVALID_DIFFICULTY", 
                    $"Invalid difficulty. Valid values are: {string.Join(", ", Enum.GetNames<CourseDifficulty>())}");
            }
        }

        // Validate instructor exists
        var instructor = await _unitOfWork.AccountRepository.GetByIdAsync(request.InstructorId);
        if (instructor == null)
            return ServiceResult<CourseDto>.Error("INSTRUCTOR_NOT_FOUND", "Instructor not found");

        // Validate category exists if provided
        if (request.CategoryId.HasValue)
        {
            var category = await _unitOfWork.Repository<Domain.Entities.CourseCategory>()
                .GetByIdAsync(request.CategoryId.Value);
            if (category == null)
                return ServiceResult<CourseDto>.Error("CATEGORY_NOT_FOUND", "Category not found");
        }

        var course = request.ToEntity();
        _unitOfWork.CourseRepository.PrepareCreate(course);
        await _unitOfWork.SaveChangesAsync();

        var createdCourse = await _unitOfWork.CourseRepository.GetByIdWithRelationsAsync(course.CourseId);
        return ServiceResult<CourseDto>.Created(createdCourse!.ToDto(), "Course created successfully");
    }

    public async Task<ServiceResult<CourseDto>> UpdateCourseAsync(Guid id, UpdateCourseRequest request)
    {
        var course = await _unitOfWork.CourseRepository.GetByIdAsync(id);
        if (course == null)
            return ServiceResult<CourseDto>.NotFound("Course not found");

        // Validate difficulty enum if provided
        if (!string.IsNullOrWhiteSpace(request.Difficulty))
        {
            if (!Enum.TryParse<CourseDifficulty>(request.Difficulty, true, out _))
            {
                return ServiceResult<CourseDto>.Error(
                    "INVALID_DIFFICULTY", 
                    $"Invalid difficulty. Valid values are: {string.Join(", ", Enum.GetNames<CourseDifficulty>())}");
            }
        }

        // Validate category exists if provided
        if (request.CategoryId.HasValue)
        {
            var category = await _unitOfWork.Repository<Domain.Entities.CourseCategory>()
                .GetByIdAsync(request.CategoryId.Value);
            if (category == null)
                return ServiceResult<CourseDto>.Error("CATEGORY_NOT_FOUND", "Category not found");
        }

        course.UpdateCourse(request);
        _unitOfWork.CourseRepository.PrepareUpdate(course);
        await _unitOfWork.SaveChangesAsync();

        var updatedCourse = await _unitOfWork.CourseRepository.GetByIdWithRelationsAsync(id);
        return ServiceResult<CourseDto>.Ok(updatedCourse!.ToDto(), "Course updated successfully");
    }

    public async Task<ServiceResult> DeleteCourseAsync(Guid id)
    {
        var course = await _unitOfWork.CourseRepository.GetByIdAsync(id);
        if (course == null)
            return ServiceResult.NotFound("Course not found");

        // Soft delete: Set IsActive to false instead of removing from database
        course.IsActive = false;
        course.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.CourseRepository.PrepareUpdate(course);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Ok("Course deleted successfully");
    }

    public async Task<ServiceResult<CourseDto>> RestoreCourseAsync(Guid id)
    {
        var course = await _unitOfWork.CourseRepository.GetByIdAsync(id);
        if (course == null)
            return ServiceResult<CourseDto>.NotFound("Course not found");

        if (course.IsActive)
            return ServiceResult<CourseDto>.Error("COURSE_ALREADY_ACTIVE", "Course is already active");

        // Restore: Set IsActive back to true
        course.IsActive = true;
        course.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.CourseRepository.PrepareUpdate(course);
        await _unitOfWork.SaveChangesAsync();

        var restoredCourse = await _unitOfWork.CourseRepository.GetByIdWithRelationsAsync(id);
        return ServiceResult<CourseDto>.Ok(restoredCourse!.ToDto(), "Course restored successfully");
    }
}

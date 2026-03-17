using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;
using Learn2Code.Application.DTOs.SectionDTOs.SectionRequests;
using Learn2Code.Application.DTOs.SectionDTOs.SectionResponses;
using Learn2Code.Application.Interfaces;
using Learn2Code.Application.Mapper;
using Learn2Code.Infrastructure.Persistence.UnitOfWork;

namespace Learn2Code.Application.Services;

public class SectionService : ISectionService
{
    private readonly IUnitOfWork _unitOfWork;

    public SectionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult<List<SectionDto>>> GetSectionsByCourseIdAsync(Guid courseId)
    {
        // Validate course exists
        var course = await _unitOfWork.CourseRepository.GetByIdAsync(courseId);
        if (course == null)
            return ServiceResult<List<SectionDto>>.NotFound("Course not found");

        var sections = await _unitOfWork.SectionRepository.GetByCourseIdAsync(courseId);
        var sectionDtos = sections.Select(s => s.ToDto()).ToList();

        return ServiceResult<List<SectionDto>>.Ok(sectionDtos);
    }

    public async Task<ServiceResult<SectionDto>> GetSectionByIdAsync(Guid sectionId)
    {
        var section = await _unitOfWork.SectionRepository.GetByIdAsync(sectionId);
        if (section == null)
            return ServiceResult<SectionDto>.NotFound("Section not found");

        return ServiceResult<SectionDto>.Ok(section.ToDto());
    }

    public async Task<ServiceResult<SectionDto>> CreateSectionAsync(Guid courseId, CreateSectionRequest request)
    {
        // Validate course exists
        var course = await _unitOfWork.CourseRepository.GetByIdAsync(courseId);
        if (course == null)
            return ServiceResult<SectionDto>.NotFound("Course not found");

        var sectionsInCourse = await _unitOfWork.SectionRepository.GetByCourseIdAsync(courseId);
        var maxPosition = sectionsInCourse.Count + 1;
        var desiredOrder = request.OrderNumber <= 0 ? maxPosition : request.OrderNumber;
        desiredOrder = Math.Max(1, Math.Min(desiredOrder, maxPosition));

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            await _unitOfWork.SectionRepository.ShiftOrderNumbersUpAsync(courseId, desiredOrder);

            var section = request.ToEntity(courseId);
            section.OrderNumber = desiredOrder;
            _unitOfWork.SectionRepository.PrepareCreate(section);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return ServiceResult<SectionDto>.Created(section.ToDto(), "Section created successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return ServiceResult<SectionDto>.Error("CREATE_SECTION_FAILED", $"Failed to create section: {ex.Message}", 500);
        }
    }

    public async Task<ServiceResult<SectionDto>> UpdateSectionAsync(Guid courseId, Guid sectionId, UpdateSectionRequest request)
    {
        // Validate course exists
        var course = await _unitOfWork.CourseRepository.GetByIdAsync(courseId);
        if (course == null)
            return ServiceResult<SectionDto>.NotFound("Course not found");

        var section = await _unitOfWork.SectionRepository.GetByIdAsync(sectionId);
        if (section == null)
            return ServiceResult<SectionDto>.NotFound("Section not found");

        // Validate section belongs to course
        if (section.CourseId != courseId)
            return ServiceResult<SectionDto>.Error("SECTION_NOT_IN_COURSE", "Section does not belong to this course");

        var sectionsInCourse = await _unitOfWork.SectionRepository.GetByCourseIdAsync(courseId);
        var totalSections = sectionsInCourse.Count;
        var currentOrder = section.OrderNumber;

        var desiredOrder = request.OrderNumber ?? currentOrder;
        desiredOrder = Math.Max(1, Math.Min(desiredOrder, totalSections));

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            if (desiredOrder != currentOrder)
            {
                const int tempOrder = 2000000000;
                await _unitOfWork.SectionRepository.MoveSectionToOrderAsync(sectionId, tempOrder);

                if (desiredOrder < currentOrder)
                {
                    await _unitOfWork.SectionRepository.ShiftOrderRangeAsync(courseId, desiredOrder, currentOrder - 1, +1);
                }
                else
                {
                    await _unitOfWork.SectionRepository.ShiftOrderRangeAsync(courseId, currentOrder + 1, desiredOrder, -1);
                }
            }

            section.UpdateSection(request);
            section.OrderNumber = desiredOrder;
            _unitOfWork.SectionRepository.PrepareUpdate(section);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return ServiceResult<SectionDto>.Ok(section.ToDto(), "Section updated successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return ServiceResult<SectionDto>.Error("UPDATE_SECTION_FAILED", $"Failed to update section: {ex.Message}", 500);
        }
    }

    public async Task<ServiceResult> DeleteSectionAsync(Guid courseId, Guid sectionId)
    {
        // Validate course exists
        var course = await _unitOfWork.CourseRepository.GetByIdAsync(courseId);
        if (course == null)
            return ServiceResult.NotFound("Course not found");

        var section = await _unitOfWork.SectionRepository.GetByIdAsync(sectionId);
        if (section == null)
            return ServiceResult.NotFound("Section not found");

        // Validate section belongs to course
        if (section.CourseId != courseId)
            return ServiceResult.Error("SECTION_NOT_IN_COURSE", "Section does not belong to this course");

        // Hard delete: Remove from database
        _unitOfWork.SectionRepository.PrepareRemove(section);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Ok("Section deleted successfully");
    }

    public async Task<ServiceResult<List<SectionDto>>> ReorderSectionsAsync(Guid courseId, ReorderSectionsRequest request)
    {
        // Validate course exists
        var course = await _unitOfWork.CourseRepository.GetByIdAsync(courseId);
        if (course == null)
            return ServiceResult<List<SectionDto>>.NotFound("Course not found");

        if (request.SectionOrders == null || !request.SectionOrders.Any())
            return ServiceResult<List<SectionDto>>.Error("EMPTY_SECTION_ORDERS", "Section orders list cannot be empty");

        // Validate all order numbers are positive and unique
        var orderNumbers = request.SectionOrders.Select(o => o.OrderNumber).ToList();
        if (orderNumbers.Any(o => o <= 0))
            return ServiceResult<List<SectionDto>>.Error("INVALID_ORDER_NUMBER", "All order numbers must be greater than 0");

        if (orderNumbers.Count != orderNumbers.Distinct().Count())
            return ServiceResult<List<SectionDto>>.Error("DUPLICATE_ORDER_IN_REQUEST", "Order numbers must be unique");

        // Get all sections to reorder
        var sectionIds = request.SectionOrders.Select(o => o.SectionId).ToList();
        var sections = new List<Domain.Entities.Section>();

        foreach (var sectionId in sectionIds)
        {
            var section = await _unitOfWork.SectionRepository.GetByIdAsync(sectionId);
            if (section == null)
                return ServiceResult<List<SectionDto>>.Error(
                    "SECTION_NOT_FOUND",
                    $"Section {sectionId} not found");

            if (section.CourseId != courseId)
                return ServiceResult<List<SectionDto>>.Error(
                    "SECTION_NOT_IN_COURSE",
                    $"Section {sectionId} does not belong to this course");

            sections.Add(section);
        }

        // Step 1: Set all order_numbers to negative temp values to avoid unique constraint conflicts
        int tempOffset = -1000;
        for (int i = 0; i < sections.Count; i++)
        {
            sections[i].OrderNumber = tempOffset - i;
            sections[i].UpdatedAt = DateTime.UtcNow;
            _unitOfWork.SectionRepository.PrepareUpdate(sections[i]);
        }

        await _unitOfWork.SaveChangesAsync();

        // Step 2: Update to final order numbers
        foreach (var orderItem in request.SectionOrders)
        {
            var section = sections.First(s => s.SectionId == orderItem.SectionId);
            section.OrderNumber = orderItem.OrderNumber;
            section.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.SectionRepository.PrepareUpdate(section);
        }

        await _unitOfWork.SaveChangesAsync();

        // Return updated sections sorted by order
        var updatedSections = await _unitOfWork.SectionRepository.GetByCourseIdAsync(courseId);
        return ServiceResult<List<SectionDto>>.Ok(
            updatedSections.Select(s => s.ToDto()).ToList(),
            "Sections reordered successfully");
    }
}

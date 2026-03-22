using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs.ExerciseMediaDTOs.ExerciseMediaRequests;
using Learn2Code.Application.DTOs.ExerciseMediaDTOs.ExerciseMediaResponses;
using Learn2Code.Application.Interfaces;
using Learn2Code.Application.Mapper;
using Learn2Code.Domain.Entities;
using Learn2Code.Domain.Enums;
using Learn2Code.Infrastructure.Persistence.UnitOfWork;

namespace Learn2Code.Application.Services;

public class ExerciseMediaService : IExerciseMediaService
{
    private readonly IUnitOfWork _unitOfWork;

    public ExerciseMediaService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult<List<MediaDetailDto>>> GetMediaByExerciseIdAsync(Guid exerciseId)
    {
        var exercise = await _unitOfWork.ExerciseRepository.GetByIdAsync(exerciseId);
        if (exercise == null)
            return ServiceResult<List<MediaDetailDto>>.NotFound("Exercise not found");

        var mediaList = await _unitOfWork.Repository<ExerciseMedia>()
            .GetAllAsync();

        var filtered = mediaList
            .Where(m => m.ExerciseId == exerciseId)
            .OrderBy(m => m.OrderNumber)
            .Select(m => m.ToMediaDetailDto())
            .ToList();

        return ServiceResult<List<MediaDetailDto>>.Ok(filtered);
    }

    public async Task<ServiceResult<MediaDetailDto>> CreateMediaAsync(Guid exerciseId, CreateExerciseMediaRequest request)
    {
        // Validate exercise exists
        var exercise = await _unitOfWork.ExerciseRepository.GetByIdAsync(exerciseId);
        if (exercise == null)
            return ServiceResult<MediaDetailDto>.NotFound("Exercise not found");

        // Only Reading exercises can have media
        if (exercise.ExerciseType != ExerciseType.Reading)
            return ServiceResult<MediaDetailDto>.Error("INVALID_EXERCISE_TYPE",
                "Media can only be added to Reading exercises");

        // Check for duplicate order_number
        var orderExists = await _unitOfWork.Repository<ExerciseMedia>()
            .AnyAsync(m => m.ExerciseId == exerciseId && m.OrderNumber == request.OrderNumber);
        if (orderExists)
            return ServiceResult<MediaDetailDto>.Error("DUPLICATE_ORDER",
                $"A media item with order_number {request.OrderNumber} already exists for this exercise");

        var media = request.ToEntity(exerciseId);
        _unitOfWork.Repository<ExerciseMedia>().PrepareCreate(media);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult<MediaDetailDto>.Created(media.ToMediaDetailDto(), "Media created successfully");
    }

    public async Task<ServiceResult> DeleteMediaAsync(Guid exerciseId, Guid mediaId)
    {
        // Validate exercise exists
        var exercise = await _unitOfWork.ExerciseRepository.GetByIdAsync(exerciseId);
        if (exercise == null)
            return ServiceResult.NotFound("Exercise not found");

        // Find the media
        var media = await _unitOfWork.Repository<ExerciseMedia>().GetByIdAsync(mediaId);
        if (media == null || media.ExerciseId != exerciseId)
            return ServiceResult.NotFound("Media not found or does not belong to this exercise");

        _unitOfWork.Repository<ExerciseMedia>().PrepareRemove(media);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Ok("Media deleted successfully");
    }
}

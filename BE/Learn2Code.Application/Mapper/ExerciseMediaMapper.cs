using Learn2Code.Application.DTOs.ExerciseMediaDTOs.ExerciseMediaRequests;
using Learn2Code.Application.DTOs.ExerciseMediaDTOs.ExerciseMediaResponses;
using Learn2Code.Domain.Entities;

namespace Learn2Code.Application.Mapper;

public static class ExerciseMediaMapper
{
    public static MediaDetailDto ToMediaDetailDto(this ExerciseMedia media)
    {
        return new MediaDetailDto
        {
            MediaId = media.MediaId,
            ExerciseId = media.ExerciseId,
            MediaType = media.MediaType.ToString(),
            Url = media.Url,
            Caption = media.Caption,
            OrderNumber = media.OrderNumber,
            CreatedAt = media.CreatedAt,
            UpdatedAt = media.UpdatedAt
        };
    }

    public static ExerciseMedia ToEntity(this CreateExerciseMediaRequest request, Guid exerciseId)
    {
        return new ExerciseMedia
        {
            MediaId = Guid.NewGuid(),
            ExerciseId = exerciseId,
            MediaType = request.MediaType,
            Url = request.Url,
            Caption = request.Caption,
            OrderNumber = request.OrderNumber,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}

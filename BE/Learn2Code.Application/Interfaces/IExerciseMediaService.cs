using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs.ExerciseMediaDTOs.ExerciseMediaRequests;
using Learn2Code.Application.DTOs.ExerciseMediaDTOs.ExerciseMediaResponses;

namespace Learn2Code.Application.Interfaces;

public interface IExerciseMediaService
{
    Task<ServiceResult<List<MediaDetailDto>>> GetMediaByExerciseIdAsync(Guid exerciseId);
    Task<ServiceResult<MediaDetailDto>> CreateMediaAsync(Guid exerciseId, CreateExerciseMediaRequest request);
    Task<ServiceResult> DeleteMediaAsync(Guid exerciseId, Guid mediaId);
}

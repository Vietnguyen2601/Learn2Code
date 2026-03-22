using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs.LeaderboardDTOs;
using Learn2Code.Application.Interfaces;
using Learn2Code.Domain.Entities;
using Learn2Code.Infrastructure.Persistence.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Learn2Code.Application.Services;

public class LeaderboardService : ILeaderboardService
{
    private readonly IUnitOfWork _unitOfWork;

    public LeaderboardService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult<List<LeaderboardEntryDto>>> GetLeaderboardAsync(Guid courseId, int top = 50)
    {
        var course = await _unitOfWork.CourseRepository.GetByIdAsync(courseId);
        if (course == null)
            return ServiceResult<List<LeaderboardEntryDto>>.NotFound("Course not found");

        var entries = await _unitOfWork.Repository<Leaderboard>()
            .GetAllQueryable()
            .Where(l => l.CourseId == courseId)
            .Include(l => l.Student)
            .OrderByDescending(l => l.TotalScore)
            .Take(top)
            .ToListAsync();

        var dtos = entries.Select((e, index) => new LeaderboardEntryDto
        {
            Rank = e.Rank ?? (index + 1),
            StudentId = e.StudentId,
            StudentName = e.Student?.Name ?? e.Student?.Username,
            TotalScore = e.TotalScore
        }).ToList();

        return ServiceResult<List<LeaderboardEntryDto>>.Ok(dtos);
    }
}

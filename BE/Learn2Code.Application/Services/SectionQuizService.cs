using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;
using Learn2Code.Application.Interfaces;
using Learn2Code.Application.Mapper;
using Learn2Code.Domain.Entities;
using Learn2Code.Domain.Enums;
using Learn2Code.Infrastructure.Persistence.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Learn2Code.Application.Services;

public class SectionQuizService : ISectionQuizService
{
    private readonly IUnitOfWork _unitOfWork;

    public SectionQuizService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult<SectionQuizDto>> GetSectionQuizAsync(Guid sectionId, Guid studentId)
    {
        // Kiểm tra section có tồn tại không
        var section = await _unitOfWork.SectionRepository.GetByIdAsync(sectionId);
        if (section == null)
            return ServiceResult<SectionQuizDto>.NotFound("Section not found");

        // Lấy tất cả lessons trong section
        var lessons = await _unitOfWork.LessonRepository.GetLessonsBySectionIdAsync(sectionId);
        if (!lessons.Any())
            return ServiceResult<SectionQuizDto>.Error("NO_LESSONS", "This section has no lessons", 404);

        // Kiểm tra tất cả LessonProgress trong section đã Completed chưa
        var lessonIds = lessons.Select(l => l.LessonId).ToList();
        var lessonProgresses = await _unitOfWork.Repository<LessonProgress>()
            .GetAllQueryable()
            .Where(lp => lp.StudentId == studentId && lessonIds.Contains(lp.LessonId))
            .ToListAsync();

        var completedLessonIds = lessonProgresses
            .Where(lp => lp.Status == LessonProgressStatus.Completed)
            .Select(lp => lp.LessonId)
            .ToHashSet();

        // Nếu chưa hoàn thành tất cả lessons
        if (completedLessonIds.Count < lessons.Count())
        {
            return ServiceResult<SectionQuizDto>.Error(
                "SECTION_NOT_COMPLETED",
                "You must complete all lessons in this section before taking the quiz",
                403);
        }

        // Lấy tất cả quizzes từ các lessons trong section
        var quizzes = await _unitOfWork.QuizRepository.GetQuizzesBySectionIdAsync(sectionId);
        if (!quizzes.Any())
            return ServiceResult<SectionQuizDto>.Error("NO_QUIZZES", "This section has no quizzes", 404);

        var dto = section.ToSectionQuizDto(quizzes);
        return ServiceResult<SectionQuizDto>.Ok(dto);
    }

    public async Task<ServiceResult<SectionQuizAttemptResultDto>> SubmitSectionQuizAttemptAsync(
        Guid sectionId, Guid studentId, SubmitSectionQuizRequest request)
    {
        // Kiểm tra section có tồn tại không
        var section = await _unitOfWork.SectionRepository.GetByIdAsync(sectionId);
        if (section == null)
            return ServiceResult<SectionQuizAttemptResultDto>.NotFound("Section not found");

        if (!request.Answers.Any())
            return ServiceResult<SectionQuizAttemptResultDto>.Error("INVALID_REQUEST", "No answers provided", 400);

        // Lấy tất cả quizzes của section
        var quizzes = await _unitOfWork.QuizRepository.GetQuizzesBySectionIdAsync(sectionId);
        var quizIds = quizzes.Select(q => q.QuizId).ToHashSet();

        // Validate tất cả quiz_id trong request phải thuộc section này
        var invalidQuizIds = request.Answers.Where(a => !quizIds.Contains(a.QuizId)).ToList();
        if (invalidQuizIds.Any())
            return ServiceResult<SectionQuizAttemptResultDto>.Error("INVALID_QUIZ_ID", "Some quiz IDs do not belong to this section", 400);

        // Lấy tất cả options để check
        var answerOptionIds = request.Answers.Select(a => a.OptionId).ToList();
        var options = await _unitOfWork.Repository<QuizOption>()
            .GetAllQueryable()
            .Where(o => answerOptionIds.Contains(o.OptionId))
            .ToListAsync();

        var optionDict = options.ToDictionary(o => o.OptionId);

        // Chấm điểm
        var totalQuestions = request.Answers.Count;
        var correctAnswers = 0;
        var answerResults = new List<SectionQuizAnswer>();

        foreach (var answer in request.Answers)
        {
            if (!optionDict.TryGetValue(answer.OptionId, out var option))
                return ServiceResult<SectionQuizAttemptResultDto>.Error("INVALID_OPTION", $"Option {answer.OptionId} not found", 400);

            if (option.QuizId != answer.QuizId)
                return ServiceResult<SectionQuizAttemptResultDto>.Error("OPTION_MISMATCH", "Option does not belong to the specified quiz", 400);

            var isCorrect = option.IsCorrect;
            if (isCorrect) correctAnswers++;

            answerResults.Add(new SectionQuizAnswer
            {
                AnswerId = Guid.NewGuid(),
                QuizId = answer.QuizId,
                OptionId = answer.OptionId,
                IsCorrect = isCorrect
            });
        }

        var score = totalQuestions > 0 ? Math.Round((decimal)correctAnswers / totalQuestions * 100, 2) : 0;
        var isPassed = score >= 70; // Pass threshold: 70%

        // Tạo attempt
        var attempt = new SectionQuizAttempt
        {
            AttemptId = Guid.NewGuid(),
            SectionId = sectionId,
            StudentId = studentId,
            Score = score,
            IsPassed = isPassed,
            AttemptedAt = DateTime.UtcNow,
            Answers = answerResults
        };

        // Gán AttemptId cho tất cả answers
        foreach (var ans in answerResults)
        {
            ans.AttemptId = attempt.AttemptId;
        }

        _unitOfWork.Repository<SectionQuizAttempt>().PrepareCreate(attempt);
        await _unitOfWork.SaveChangesAsync();

        // Load lại attempt với navigation properties để map sang DTO
        var savedAttempt = _unitOfWork.Repository<SectionQuizAttempt>()
            .GetAllQueryable()
            .Include(a => a.Answers)
                .ThenInclude(ans => ans.Quiz)
            .FirstOrDefault(a => a.AttemptId == attempt.AttemptId);

        if (savedAttempt == null)
            return ServiceResult<SectionQuizAttemptResultDto>.Error("SAVE_FAILED", "Failed to save attempt", 500);

        var resultDto = savedAttempt.ToResultDto();
        return ServiceResult<SectionQuizAttemptResultDto>.Ok(resultDto, "Quiz submitted successfully");
    }

    public async Task<ServiceResult<List<SectionQuizAttemptResultDto>>> GetStudentAttemptsAsync(Guid sectionId, Guid studentId)
    {
        // Kiểm tra section có tồn tại không
        var section = await _unitOfWork.SectionRepository.GetByIdAsync(sectionId);
        if (section == null)
            return ServiceResult<List<SectionQuizAttemptResultDto>>.NotFound("Section not found");

        var attempts = await _unitOfWork.Repository<SectionQuizAttempt>()
            .GetAllQueryable()
            .Include(a => a.Answers)
                .ThenInclude(ans => ans.Quiz)
            .Where(a => a.SectionId == sectionId && a.StudentId == studentId)
            .OrderByDescending(a => a.AttemptedAt)
            .ToListAsync();

        var dtos = attempts.Select(a => a.ToResultDto()).ToList();
        return ServiceResult<List<SectionQuizAttemptResultDto>>.Ok(dtos);
    }
}

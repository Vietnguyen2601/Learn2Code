using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;
using Learn2Code.Application.Interfaces;
using Learn2Code.Application.Mapper;
using Learn2Code.Domain.Entities;
using Learn2Code.Domain.Enums;
using Learn2Code.Infrastructure.Persistence.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Learn2Code.Application.Services;

public class QuizService : IQuizService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<QuizService> _logger;

    public QuizService(IUnitOfWork unitOfWork, ILogger<QuizService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ServiceResult<AnswerQuizResponse>> AnswerQuizAsync(
        Guid quizId,
        Guid studentId,
        AnswerQuizRequest request)
    {
        // Get quiz with options
        var quiz = await _unitOfWork.QuizRepository.GetWithOptionsAsync(quizId);

        if (quiz == null)
        {
            return ServiceResult<AnswerQuizResponse>.NotFound("Quiz not found");
        }

        // Find selected option
        var selectedOption = quiz.Options.FirstOrDefault(o => o.OptionId == request.OptionId);

        if (selectedOption == null)
        {
            return ServiceResult<AnswerQuizResponse>.Error(
                "INVALID_OPTION",
                "Invalid option selected",
                400);
        }

        // Find correct option
        var correctOption = quiz.Options.FirstOrDefault(o => o.IsCorrect);

        if (correctOption == null)
        {
            _logger.LogError("Quiz {QuizId} has no correct answer defined", quizId);
            return ServiceResult<AnswerQuizResponse>.Error(
                "QUIZ_CONFIGURATION_ERROR",
                "Quiz configuration error",
                500);
        }

        var response = new AnswerQuizResponse
        {
            QuizId = quiz.QuizId,
            IsCorrect = selectedOption.IsCorrect,
            Explanation = quiz.Explanation,
            CorrectOptionId = correctOption.OptionId
        };

        return ServiceResult<AnswerQuizResponse>.Ok(response);
    }

    public async Task<ServiceResult<SectionQuizDto>> GetSectionQuizAsync(
        Guid sectionId,
        Guid studentId)
    {
        // Get section
        var section = await _unitOfWork.Repository<Section>().GetByIdAsync(sectionId);

        if (section == null)
        {
            return ServiceResult<SectionQuizDto>.NotFound("Section not found");
        }

        // Get all lessons in section
        var lessons = await _unitOfWork.LessonRepository.GetBySectionIdAsync(sectionId);

        if (!lessons.Any())
        {
            return ServiceResult<SectionQuizDto>.Error(
                "NO_LESSONS",
                "Section has no lessons",
                400);
        }

        // Check if all lessons are completed
        var lessonProgresses = await _unitOfWork.LessonProgressRepository
            .GetByStudentAndCourseAsync(studentId, section.CourseId);

        var allLessonsCompleted = lessons.All(lesson =>
            lessonProgresses.Any(lp =>
                lp.LessonId == lesson.LessonId &&
                lp.Status == LessonProgressStatus.Completed));

        if (!allLessonsCompleted)
        {
            var response = new SectionQuizDto
            {
                SectionId = sectionId,
                SectionTitle = section.Title,
                TotalQuestions = 0,
                IsUnlocked = false,
                UnlockMessage = "You must complete all lessons in this section before taking the section quiz",
                Quizzes = new List<QuizDto>()
            };

            return ServiceResult<SectionQuizDto>.Ok(response);
        }

        // Get all quizzes from all lessons in section
        var quizzes = await _unitOfWork.QuizRepository.GetBySectionIdAsync(sectionId);

        if (!quizzes.Any())
        {
            return ServiceResult<SectionQuizDto>.Error(
                "NO_QUIZZES",
                "Section has no quizzes",
                400);
        }

        var quizDtos = quizzes.Select(q => q.ToQuizDto(includeCorrectAnswer: false)).ToList();

        var sectionQuizDto = new SectionQuizDto
        {
            SectionId = sectionId,
            SectionTitle = section.Title,
            TotalQuestions = quizzes.Count,
            IsUnlocked = true,
            UnlockMessage = null,
            Quizzes = quizDtos
        };

        return ServiceResult<SectionQuizDto>.Ok(sectionQuizDto);
    }

    public async Task<ServiceResult<SubmitSectionQuizResponse>> SubmitSectionQuizAsync(
        Guid sectionId,
        Guid studentId,
        SubmitSectionQuizRequest request)
    {
        // Validate section
        var section = await _unitOfWork.Repository<Section>().GetByIdAsync(sectionId);

        if (section == null)
        {
            return ServiceResult<SubmitSectionQuizResponse>.NotFound("Section not found");
        }

        // Get all quizzes in section
        var quizzes = await _unitOfWork.QuizRepository.GetBySectionIdAsync(sectionId);

        if (!quizzes.Any())
        {
            return ServiceResult<SubmitSectionQuizResponse>.Error(
                "NO_QUIZZES",
                "Section has no quizzes",
                400);
        }

        // Validate all answers are for quizzes in this section
        var quizIds = quizzes.Select(q => q.QuizId).ToHashSet();
        var invalidQuizzes = request.Answers.Where(a => !quizIds.Contains(a.QuizId)).ToList();

        if (invalidQuizzes.Any())
        {
            return ServiceResult<SubmitSectionQuizResponse>.Error(
                "INVALID_QUIZ_IDS",
                "Some quiz IDs are not part of this section",
                400);
        }

        // Grade answers
        var answerResults = new List<SectionQuizAnswerResult>();
        var correctCount = 0;

        foreach (var answer in request.Answers)
        {
            var quiz = quizzes.First(q => q.QuizId == answer.QuizId);
            var selectedOption = quiz.Options.FirstOrDefault(o => o.OptionId == answer.OptionId);
            var correctOption = quiz.Options.First(o => o.IsCorrect);

            var isCorrect = selectedOption?.IsCorrect ?? false;
            if (isCorrect) correctCount++;

            answerResults.Add(new SectionQuizAnswerResult
            {
                QuizId = quiz.QuizId,
                Question = quiz.Question,
                IsCorrect = isCorrect,
                SelectedOptionId = answer.OptionId,
                CorrectOptionId = correctOption.OptionId,
                Explanation = quiz.Explanation
            });
        }

        // Calculate score
        var score = quizzes.Count > 0 ? (decimal)correctCount / quizzes.Count * 100 : 0;

        // Determine pass/fail (default: 70% to pass)
        var isPassed = score >= 70;

        // Create attempt record
        var attempt = new SectionQuizAttempt
        {
            AttemptId = Guid.NewGuid(),
            SectionId = sectionId,
            StudentId = studentId,
            Score = score,
            IsPassed = isPassed,
            AttemptedAt = DateTime.UtcNow
        };

        _unitOfWork.Repository<SectionQuizAttempt>().PrepareCreate(attempt);

        // Save individual answers
        foreach (var answer in request.Answers)
        {
            var quiz = quizzes.First(q => q.QuizId == answer.QuizId);
            var selectedOption = quiz.Options.FirstOrDefault(o => o.OptionId == answer.OptionId);

            var sectionQuizAnswer = new SectionQuizAnswer
            {
                AnswerId = Guid.NewGuid(),
                AttemptId = attempt.AttemptId,
                QuizId = answer.QuizId,
                OptionId = answer.OptionId,
                IsCorrect = selectedOption?.IsCorrect ?? false
            };

            _unitOfWork.Repository<SectionQuizAnswer>().PrepareCreate(sectionQuizAnswer);
        }

        await _unitOfWork.CommitTransactionAsync();

        // Check if certification should be issued
        var certificationIssued = false;
        string? certificateCode = null;

        if (isPassed)
        {
            var certResult = await CheckAndIssueCertificationAsync(studentId, section.CourseId);
            certificationIssued = certResult.issued;
            certificateCode = certResult.code;
        }

        var response = new SubmitSectionQuizResponse
        {
            AttemptId = attempt.AttemptId,
            Score = score,
            IsPassed = isPassed,
            TotalQuestions = quizzes.Count,
            CorrectAnswers = correctCount,
            AttemptedAt = attempt.AttemptedAt,
            Answers = answerResults,
            CertificationIssued = certificationIssued,
            CertificateCode = certificateCode
        };

        return ServiceResult<SubmitSectionQuizResponse>.Ok(response);
    }

    public async Task<ServiceResult<SectionQuizAttemptListDto>> GetMyAttemptsAsync(
        Guid sectionId,
        Guid studentId)
    {
        var attempts = await _unitOfWork.SectionQuizAttemptRepository
            .GetByStudentAndSectionAsync(studentId, sectionId);

        var attemptDtos = attempts.Select(a =>
        {
            var correctAnswers = a.Answers.Count(ans => ans.IsCorrect);
            var totalQuestions = a.Answers.Count;

            return a.ToSectionQuizAttemptDto(totalQuestions, correctAnswers);
        }).ToList();

        var bestScore = attemptDtos.Any() ? attemptDtos.Max(a => a.Score) : (decimal?)null;
        var bestAttempt = attemptDtos.FirstOrDefault(a => a.Score == bestScore);

        var result = new SectionQuizAttemptListDto
        {
            Attempts = attemptDtos,
            TotalCount = attemptDtos.Count,
            BestScore = bestScore,
            BestAttemptId = bestAttempt?.AttemptId
        };

        return ServiceResult<SectionQuizAttemptListDto>.Ok(result);
    }

    #region Private Helper Methods

    /// <summary>
    /// Check course completion rules and issue certification if qualified
    /// </summary>
    private async Task<(bool issued, string? code)> CheckAndIssueCertificationAsync(
        Guid studentId,
        Guid courseId)
    {
        // Check if student already has certification
        var existingCert = await _unitOfWork.Repository<Certification>()
            .GetAsync(c => c.StudentId == studentId && c.CourseId == courseId);

        if (existingCert != null)
        {
            return (false, null); // Already certified
        }

        // Get completion rules
        var rules = await _unitOfWork.Repository<CourseCompletionRule>()
            .GetAsync(r => r.CourseId == courseId);

        if (rules == null)
        {
            _logger.LogWarning("No completion rules found for course {CourseId}", courseId);
            return (false, null);
        }

        // Check lesson completion
        var allLessons = await _unitOfWork.Repository<Lesson>()
            .GetAllQueryable()
            .Where(l => l.Section.CourseId == courseId)
            .ToListAsync();

        var lessonProgresses = await _unitOfWork.LessonProgressRepository
            .GetByStudentAndCourseAsync(studentId, courseId);

        var completedLessonsCount = lessonProgresses.Count(lp => lp.Status == LessonProgressStatus.Completed);
        var lessonCompletionPct = allLessons.Any() ? (decimal)completedLessonsCount / allLessons.Count() * 100 : 0;

        if (lessonCompletionPct < rules.MinLessonCompletionPct)
        {
            return (false, null);
        }

        // Check section quiz requirements
        if (rules.RequireAllSectionQuiz)
        {
            var sections = await _unitOfWork.Repository<Section>()
                .GetAllQueryable()
                .Where(s => s.CourseId == courseId)
                .ToListAsync();

            foreach (var section in sections)
            {
                var sectionAttempts = await _unitOfWork.SectionQuizAttemptRepository
                    .GetByStudentAndSectionAsync(studentId, section.SectionId);

                var bestAttempt = sectionAttempts
                    .Where(a => a.IsPassed && a.Score >= rules.MinSectionQuizScore)
                    .OrderByDescending(a => a.Score)
                    .FirstOrDefault();

                if (bestAttempt == null)
                {
                    return (false, null); // Not all sections passed
                }
            }
        }

        // Issue certification
        var certificateCode = GenerateCertificateCode();

        var certification = new Certification
        {
            CertificationId = Guid.NewGuid(),
            StudentId = studentId,
            CourseId = courseId,
            CertificateCode = certificateCode,
            IssuedAt = DateTime.UtcNow
        };

        _unitOfWork.Repository<Certification>().PrepareCreate(certification);
        await _unitOfWork.CommitTransactionAsync();

        _logger.LogInformation(
            "Certification {Code} issued to student {StudentId} for course {CourseId}",
            certificateCode,
            studentId,
            courseId);

        return (true, certificateCode);
    }

    private static string GenerateCertificateCode()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var random = new Random().Next(1000, 9999);
        return $"CERT-{timestamp}-{random}";
    }

    #endregion
}

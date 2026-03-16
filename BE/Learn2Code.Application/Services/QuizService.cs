using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;
using Learn2Code.Application.Interfaces;
using Learn2Code.Application.Mapper;
using Learn2Code.Infrastructure.Persistence.UnitOfWork;

namespace Learn2Code.Application.Services;

public class QuizService : IQuizService
{
    private readonly IUnitOfWork _unitOfWork;

    public QuizService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult<List<QuizDto>>> GetQuizzesByLessonIdAsync(Guid lessonId)
    {
        // Ki?m tra lesson c� t?n t?i kh�ng
        var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(lessonId);
        if (lesson == null)
            return ServiceResult<List<QuizDto>>.NotFound("Lesson not found");

        var quizzes = await _unitOfWork.QuizRepository.GetQuizzesByLessonIdAsync(lessonId);
        var quizDtos = quizzes.Select(q => q.ToDto()).ToList();

        return ServiceResult<List<QuizDto>>.Ok(quizDtos);
    }

    public async Task<ServiceResult<QuizDto>> CreateQuizAsync(Guid lessonId, CreateQuizRequest request)
    {
        // Ki?m tra lesson c� t?n t?i kh�ng
        var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(lessonId);
        if (lesson == null)
            return ServiceResult<QuizDto>.NotFound("Lesson not found");

        // Validate: Ph?i c� �t nh?t 2 options
        if (request.Options == null || request.Options.Count < 2)
            return ServiceResult<QuizDto>.Error("INVALID_OPTIONS", "Quiz must have at least 2 options");

        // Validate: Ph?i c� �t nh?t 1 ?�p �n ?�ng
        if (!request.Options.Any(o => o.IsCorrect))
            return ServiceResult<QuizDto>.Error("NO_CORRECT_ANSWER", "Quiz must have at least 1 correct answer");

        // L?y order number ti?p theo
        var maxOrder = await _unitOfWork.QuizRepository.GetMaxOrderNumberInLessonAsync(lessonId);
        var newOrderNumber = maxOrder + 1;

        // T?o Quiz
        var quiz = request.ToEntity(lessonId, newOrderNumber);
        _unitOfWork.QuizRepository.PrepareCreate(quiz);

        // T?o Options
        foreach (var optionRequest in request.Options)
        {
            var option = optionRequest.ToOptionEntity(quiz.QuizId);
            _unitOfWork.QuizOptionRepository.PrepareCreate(option);
        }

        await _unitOfWork.SaveChangesAsync();

        // Load l?i quiz v?i options
        var createdQuiz = await _unitOfWork.QuizRepository.GetQuizWithOptionsAsync(quiz.QuizId);
        return ServiceResult<QuizDto>.Created(createdQuiz!.ToDto(), "Quiz created successfully");
    }

    public async Task<ServiceResult<QuizDto>> UpdateQuizAsync(Guid quizId, UpdateQuizRequest request)
    {
        var quiz = await _unitOfWork.QuizRepository.GetQuizWithOptionsAsync(quizId);
        if (quiz == null)
            return ServiceResult<QuizDto>.NotFound("Quiz not found");

        // C?p nh?t quiz
        quiz.UpdateQuiz(request);
        _unitOfWork.QuizRepository.PrepareUpdate(quiz);

        // N?u c� update options
        if (request.Options != null && request.Options.Count > 0)
        {
            // Validate: Ph?i c� �t nh?t 2 options
            if (request.Options.Count < 2)
                return ServiceResult<QuizDto>.Error("INVALID_OPTIONS", "Quiz must have at least 2 options");

            // Validate: Ph?i c� �t nh?t 1 ?�p �n ?�ng
            if (!request.Options.Any(o => o.IsCorrect))
                return ServiceResult<QuizDto>.Error("NO_CORRECT_ANSWER", "Quiz must have at least 1 correct answer");

            // X�a t?t c? options c?
            await _unitOfWork.QuizOptionRepository.DeleteOptionsByQuizIdAsync(quizId);

            // T?o options m?i t? request
            foreach (var optionRequest in request.Options)
            {
                var newOption = new Domain.Entities.QuizOption
                {
                    OptionId = Guid.NewGuid(),
                    QuizId = quizId,
                    Content = optionRequest.Content,
                    IsCorrect = optionRequest.IsCorrect,
                    CreatedAt = DateTime.UtcNow
                };
                _unitOfWork.QuizOptionRepository.PrepareCreate(newOption);
            }
        }

        await _unitOfWork.SaveChangesAsync();

        // Load l?i quiz v?i options
        var updatedQuiz = await _unitOfWork.QuizRepository.GetQuizWithOptionsAsync(quizId);
        return ServiceResult<QuizDto>.Ok(updatedQuiz!.ToDto(), "Quiz updated successfully");
    }

    public async Task<ServiceResult> DeleteQuizAsync(Guid quizId)
    {
        var quiz = await _unitOfWork.QuizRepository.GetByIdAsync(quizId);
        if (quiz == null)
            return ServiceResult.NotFound("Quiz not found");

        // X�a options tr??c (cascade s? t? ??ng x�a n?u c� config, nh?ng ?? ch?c ch?n)
        await _unitOfWork.QuizOptionRepository.DeleteOptionsByQuizIdAsync(quizId);

        // X�a quiz
        _unitOfWork.QuizRepository.PrepareRemove(quiz);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Ok("Quiz deleted successfully");
    }

    public async Task<ServiceResult<QuizOptionDto>> UpdateQuizOptionAsync(Guid quizId, Guid optionId, UpdateSingleQuizOptionRequest request)
    {
        // Ki?m tra quiz c� t?n t?i kh�ng
        var quiz = await _unitOfWork.QuizRepository.GetByIdAsync(quizId);
        if (quiz == null)
            return ServiceResult<QuizOptionDto>.NotFound("Quiz not found");

        // Ki?m tra option c� t?n t?i v� thu?c quiz n�y kh�ng
        var option = await _unitOfWork.QuizOptionRepository.GetOptionByIdAsync(quizId, optionId);
        if (option == null)
            return ServiceResult<QuizOptionDto>.NotFound("Quiz option not found or does not belong to this quiz");

        // C?p nh?t option
        option.UpdateOption(request);
        _unitOfWork.QuizOptionRepository.PrepareUpdate(option);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult<QuizOptionDto>.Ok(option.ToOptionDto(), "Quiz option updated successfully");
    }

    public async Task<ServiceResult> DeleteQuizOptionAsync(Guid quizId, Guid optionId)
    {
        // Ki?m tra quiz c� t?n t?i kh�ng
        var quiz = await _unitOfWork.QuizRepository.GetByIdAsync(quizId);
        if (quiz == null)
            return ServiceResult.NotFound("Quiz not found");

        // Ki?m tra option c� t?n t?i v� thu?c quiz n�y kh�ng
        var option = await _unitOfWork.QuizOptionRepository.GetOptionByIdAsync(quizId, optionId);
        if (option == null)
            return ServiceResult.NotFound("Quiz option not found or does not belong to this quiz");

        // Ki?m tra sau khi x�a ph?i c�n �t nh?t 2 options
        var currentOptionCount = await _unitOfWork.QuizOptionRepository.CountOptionsByQuizIdAsync(quizId);
        if (currentOptionCount <= 2)
            return ServiceResult.Error("MINIMUM_OPTIONS_REQUIRED", "Quiz must have at least 2 options. Cannot delete this option.");

        // Ki?m tra n?u x�a option ?�ng, ph?i c�n �t nh?t 1 option ?�ng kh�c
        if (option.IsCorrect)
        {
            var otherCorrectOptions = await _unitOfWork.QuizOptionRepository
                .GetAsync(o => o.QuizId == quizId && o.OptionId != optionId && o.IsCorrect);
            
            if (otherCorrectOptions == null)
                return ServiceResult.Error("CORRECT_ANSWER_REQUIRED", "Quiz must have at least 1 correct answer. Cannot delete the only correct option.");
        }

        _unitOfWork.QuizOptionRepository.PrepareRemove(option);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Ok("Quiz option deleted successfully");
    }

    public async Task<ServiceResult<AnswerQuizResultDto>> AnswerQuizAsync(Guid quizId, Guid studentId, AnswerQuizRequest request)
    {
        // Kiểm tra quiz có tồn tại không
        var quiz = await _unitOfWork.QuizRepository.GetQuizWithOptionsAsync(quizId);
        if (quiz == null)
            return ServiceResult<AnswerQuizResultDto>.NotFound("Quiz not found");

        // Kiểm tra option có tồn tại và thuộc quiz này không
        var selectedOption = quiz.Options.FirstOrDefault(o => o.OptionId == request.OptionId);
        if (selectedOption == null)
            return ServiceResult<AnswerQuizResultDto>.Error("INVALID_OPTION", "Selected option does not belong to this quiz", 400);

        // Kiểm tra quyền truy cập lesson của quiz
        var canAccess = await _unitOfWork.LessonRepository.CanUserAccessLessonAsync(quiz.LessonId, studentId);
        if (!canAccess)
            return ServiceResult<AnswerQuizResultDto>.Error("ACCESS_DENIED", "You don't have permission to access this quiz", 403);

        var resultDto = new AnswerQuizResultDto
        {
            QuizId = quizId,
            OptionId = request.OptionId,
            IsCorrect = selectedOption.IsCorrect,
            Explanation = quiz.Explanation
        };

        return ServiceResult<AnswerQuizResultDto>.Ok(resultDto);
    }
}

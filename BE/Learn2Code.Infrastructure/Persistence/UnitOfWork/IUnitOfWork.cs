using Learn2Code.Infrastructure.Repositories.Base;
using Learn2Code.Infrastructure.Repositories.IRepository;

namespace Learn2Code.Infrastructure.Persistence.UnitOfWork;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    IGenericRepository<T> Repository<T>() where T : class;

    IAccountRepository AccountRepository { get; }
    IRoleRepository RoleRepository { get; }
    IEnrollmentRepository EnrollmentRepository { get; }
    ILessonRepository LessonRepository { get; }
    IExerciseRepository ExerciseRepository { get; }
    ILessonProgressRepository LessonProgressRepository { get; }
    IExerciseProgressRepository ExerciseProgressRepository { get; }
    IQuizRepository QuizRepository { get; }
    ISectionQuizAttemptRepository SectionQuizAttemptRepository { get; }

    int SaveChanges();
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}

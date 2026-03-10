using Learn2Code.Infrastructure.Repositories.Base;
using Learn2Code.Infrastructure.Repositories.IRepository;

namespace Learn2Code.Infrastructure.Persistence.UnitOfWork;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    IGenericRepository<T> Repository<T>() where T : class;

    IAccountRepository AccountRepository { get; }
    IRoleRepository RoleRepository { get; }
    ICourseRepository CourseRepository { get; }
    ICourseCategoryRepository CourseCategoryRepository { get; }
    ISectionRepository SectionRepository { get; }
    ILessonRepository LessonRepository { get; }
    IExerciseRepository ExerciseRepository { get; }
    ITestCaseRepository TestCaseRepository { get; }
    IQuizRepository QuizRepository { get; }
    IQuizOptionRepository QuizOptionRepository { get; }
    ISubscriptionPackageRepository SubscriptionPackageRepository { get; }
    ISubscriptionRepository SubscriptionRepository { get; }
    IPaymentRepository PaymentRepository { get; }
    IEnrollmentRepository EnrollmentRepository { get; }

    int SaveChanges();
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}

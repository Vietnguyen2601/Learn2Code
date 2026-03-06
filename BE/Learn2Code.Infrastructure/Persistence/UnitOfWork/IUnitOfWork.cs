using Learn2Code.Infrastructure.Repositories.Base;
using Learn2Code.Infrastructure.Repositories.IRepository;

namespace Learn2Code.Infrastructure.Persistence.UnitOfWork;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    IGenericRepository<T> Repository<T>() where T : class;

    IAccountRepository AccountRepository { get; }
    IRoleRepository RoleRepository { get; }
    ISubscriptionPackageRepository SubscriptionPackageRepository { get; }
    ISubscriptionRepository SubscriptionRepository { get; }
    IPaymentRepository PaymentRepository { get; }

    int SaveChanges();
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}

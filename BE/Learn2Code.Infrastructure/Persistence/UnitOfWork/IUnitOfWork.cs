namespace Learn2Code.Infrastructure.Persistence.UnitOfWork;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    /// Get repository for entity type T
    // IGenericRepository<T> Repository<T>() where T : class;
    /// Save all pending changes synchronously
    // int SaveChanges();
    // /// Save all pending changes asynchronously
    // Task<int> SaveChangesAsync();
    // /// Begin a database transaction
    // Task BeginTransactionAsync();
    // /// Commit the current transaction
    // Task CommitTransactionAsync();
    // /// Rollback the current transaction
    // Task RollbackTransactionAsync();
}

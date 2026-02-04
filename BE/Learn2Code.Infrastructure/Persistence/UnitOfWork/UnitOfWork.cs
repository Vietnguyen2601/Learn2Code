using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
// using Learn2Code.Infrastructure.Data.Context;
using Learn2Code.Infrastructure.Repositories.Base;

namespace Learn2Code.Infrastructure.Persistence.UnitOfWork;

public class UnitOfWork
{
    // private readonly ApplicationDbContext _context;
    // private IDbContextTransaction _transaction;
    // private readonly Dictionary<string, object> _repositories;

    // public UnitOfWork(ApplicationDbContext context)
    // {
    //     _context = context;
    //     _repositories = new Dictionary<string, object>();
    // }

    // /// Get or create a repository for entity type T
    // public IGenericRepository<T> Repository<T>() where T : class
    // {
    //     var key = typeof(T).Name;

    //     if (!_repositories.ContainsKey(key))
    //     {
    //         var repositoryType = typeof(GenericRepository<>).MakeGenericType(typeof(T));
    //         var repositoryInstance = Activator.CreateInstance(repositoryType, _context);
    //         _repositories.Add(key, repositoryInstance);
    //     }

    //     return (IGenericRepository<T>)_repositories[key];
    // }

    // /// Save all pending changes synchronously
    // public int SaveChanges()
    // {
    //     return _context.SaveChanges();
    // }

    // /// Save all pending changes asynchronously
    // public async Task<int> SaveChangesAsync()
    // {
    //     return await _context.SaveChangesAsync();
    // }

    // /// Begin a database transaction
    // public async Task BeginTransactionAsync()
    // {
    //     _transaction = await _context.Database.BeginTransactionAsync();
    // }

    // /// Commit the current transaction
    // public async Task CommitTransactionAsync()
    // {
    //     try
    //     {
    //         await SaveChangesAsync();
    //         await _transaction?.CommitAsync();
    //     }
    //     catch
    //     {
    //         await RollbackTransactionAsync();
    //         throw;
    //     }
    //     finally
    //     {
    //         // await _transaction?.DisposeAsync();
    //         _transaction = null;
    //     }
    // }

    // /// Rollback the current transaction
    // public async Task RollbackTransactionAsync()
    // {
    //     try
    //     {
    //         await _transaction?.RollbackAsync();
    //     }
    //     finally
    //     {
    //         // await _transaction?.DisposeAsync();
    //         _transaction = null;
    //     }
    // }

    // /// Dispose resources
    // public void Dispose()
    // {
    //     _transaction?.Dispose();
    //     _context?.Dispose();
    // }

    // /// Dispose resources asynchronously
    // public async ValueTask DisposeAsync()
    // {
    //     if (_transaction != null)
    //     {
    //         await _transaction.DisposeAsync();
    //     }

    //     if (_context != null)
    //     {
    //         await _context.DisposeAsync();
    //     }
    // }
}

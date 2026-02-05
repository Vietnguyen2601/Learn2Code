using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using Learn2Code.Infrastructure.Repositories.IRepository;
using Learn2Code.Infrastructure.Data.Context;
using Learn2Code.Infrastructure.Repositories.Repository;
using Learn2Code.Infrastructure.Repositories.Base;

namespace Learn2Code.Infrastructure.Persistence.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly Learn2CodeDbContext _context;
    private IDbContextTransaction? _transaction;
    private readonly Dictionary<string, object> _repositories;

    public UnitOfWork(Learn2CodeDbContext context)
    {
        _context = context;
        _repositories = new Dictionary<string, object>();
    }

    private IAccountRepository? _accountRepository;
    public IAccountRepository AccountRepository
    {
        get
        {
            return _accountRepository ??= new AccountRepository(_context);
        }
    }

    private IRoleRepository? _roleRepository;
    public IRoleRepository RoleRepository
    {
        get
        {
            return _roleRepository ??= new RoleRepository(_context);
        }
    }

    public IGenericRepository<T> Repository<T>() where T : class
    {
        var key = typeof(T).Name;

        if (!_repositories.ContainsKey(key))
        {
            var repositoryType = typeof(GenericRepository<>).MakeGenericType(typeof(T));
            var repositoryInstance = Activator.CreateInstance(repositoryType, _context);
            _repositories.Add(key, repositoryInstance!);
        }

        return (IGenericRepository<T>)_repositories[key];
    }

    public int SaveChanges()
    {
        return _context.SaveChanges();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await SaveChangesAsync();
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
            }
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
            }
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        try
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
            }
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
            }
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
        }
        await _context.DisposeAsync();
    }
}

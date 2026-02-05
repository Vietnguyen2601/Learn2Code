namespace Learn2Code.Infrastructure.Repositories.Base;

public interface IGenericRepository<T> where T : class
{
    /// <summary>
    /// Get all entities synchronously
    /// </summary>
    List<T> GetAll();

    /// <summary>
    /// Get all entities asynchronously
    /// </summary>
    Task<List<T>> GetAllAsync();

    /// <summary>
    /// Create a new entity synchronously and save changes
    /// </summary>
    void Create(T entity);

    /// <summary>
    /// Create a new entity asynchronously and save changes
    /// </summary>
    Task<int> CreateAsync(T entity);

    /// <summary>
    /// Update an entity synchronously and save changes
    /// </summary>
    void Update(T entity);

    /// <summary>
    /// Update an entity asynchronously and save changes
    /// </summary>
    Task<int> UpdateAsync(T entity);

    /// <summary>
    /// Remove an entity synchronously and save changes
    /// </summary>
    bool Remove(T entity);

    /// <summary>
    /// Remove an entity asynchronously and save changes
    /// </summary>
    Task<bool> RemoveAsync(T entity);

    /// <summary>
    /// Get entity by integer ID synchronously
    /// </summary>
    T GetById(int id);

    /// <summary>
    /// Get entity by integer ID asynchronously
    /// </summary>
    Task<T> GetByIdAsync(int id);

    /// <summary>
    /// Get entity by string code synchronously
    /// </summary>
    T GetById(string code);

    /// <summary>
    /// Get entity by string code asynchronously
    /// </summary>
    Task<T> GetByIdAsync(string code);

    /// <summary>
    /// Get entity by GUID synchronously
    /// </summary>
    T GetById(Guid code);

    /// <summary>
    /// Get entity by GUID asynchronously
    /// </summary>
    Task<T> GetByIdAsync(Guid code);

    /// <summary>
    /// Prepare entity for creation without saving
    /// </summary>
    void PrepareCreate(T entity);

    /// <summary>
    /// Prepare entity for update without saving
    /// </summary>
    void PrepareUpdate(T entity);

    /// <summary>
    /// Prepare entity for removal without saving
    /// </summary>
    void PrepareRemove(T entity);

    /// <summary>
    /// Save all pending changes synchronously
    /// </summary>
    int Save();

    /// <summary>
    /// Save all pending changes asynchronously
    /// </summary>
    Task<int> SaveAsync();

    /// <summary>
    /// Get all entities as queryable without materializing
    /// </summary>
    IQueryable<T> GetAllQueryable();
}

namespace Nimble.Modulith.Customers.Domain.Interfaces;

public interface IReadRepository<T> where T : class
{
    Task<T?> GetByIdAsync<TId>(TId id, CancellationToken ct) where TId : notnull;
}

public interface IRepository<T> : IReadRepository<T> where T : class
{
    Task AddAsync(T entity, CancellationToken ct);
    Task UpdateAsync(T entity, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
using Microsoft.EntityFrameworkCore;
using Nimble.Modulith.Customers.Domain.Interfaces;

namespace Nimble.Modulith.Customers.Data;

public class EfRepository<T>(CustomersDbContext dbContext) : IRepository<T> where T : class
{
    public async Task<T?> GetByIdAsync<TId>(TId id, CancellationToken ct) where TId : notnull
    {
        return await dbContext.Set<T>().FindAsync([id], ct);
    }

    public async Task AddAsync(T entity, CancellationToken ct)
    {
        await dbContext.Set<T>().AddAsync(entity, ct);
    }

    public async Task UpdateAsync(T entity, CancellationToken ct)
    {
        dbContext.Set<T>().Update(entity);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await dbContext.SaveChangesAsync(ct);
    }
}
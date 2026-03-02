using Microsoft.EntityFrameworkCore;
using MoneyTrace.Application.Data;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Infrastructure.Data;

public class Repository<T> : IRepository<T> where T : Entity
{
    protected readonly MoneyTraceDbContext DbContext;

    public Repository(MoneyTraceDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<T>().FirstOrDefaultAsync(e => e.Id == id && e.IsActive, cancellationToken);
    }

    public async Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<T>().Where(e => e.IsActive).ToListAsync(cancellationToken);
    }

    public void Add(T entity)
    {
        DbContext.Set<T>().Add(entity);
    }

    public void Update(T entity)
    {
        DbContext.Set<T>().Update(entity);
    }

    public void Remove(T entity)
    {
        entity.SoftDelete();
        DbContext.Set<T>().Update(entity);
    }
}

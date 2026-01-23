using Microsoft.EntityFrameworkCore;
using EvuEase.Infrastructure.Persistence;

namespace EvuEase.Infrastructure.Repositories
{
    public abstract class BaseRepository<T> where T : class
    {
        protected readonly AppDbContext dbContext;

        protected BaseRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        protected IQueryable<T> GetAll() => dbContext.Set<T>().AsQueryable();

        protected async Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await dbContext.Set<T>().ToListAsync(cancellationToken);
        }

        protected async Task<T?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            return await dbContext.Set<T>().FindAsync(new[] { id }, cancellationToken);
        }

        protected async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await dbContext.Set<T>().AddAsync(entity, cancellationToken);
            return entity;
        }

        protected void Update(T entity)
        {
            dbContext.Set<T>().Update(entity);
        }

        protected void Delete(T entity)
        {
            dbContext.Set<T>().Remove(entity);
        }

        protected async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}


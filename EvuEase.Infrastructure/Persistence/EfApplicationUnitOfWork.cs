using EvuEase.Application.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace EvuEase.Infrastructure.Persistence;

public sealed class EfApplicationUnitOfWork : IApplicationUnitOfWork
{
    private readonly AppDbContext _dbContext;

    public EfApplicationUnitOfWork(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IApplicationDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        var tx = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        return new EfDbTransactionWrapper(tx);
    }

    private sealed class EfDbTransactionWrapper : IApplicationDbTransaction
    {
        private readonly IDbContextTransaction _transaction;
        private bool _committed;

        public EfDbTransactionWrapper(IDbContextTransaction transaction)
        {
            _transaction = transaction;
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await _transaction.CommitAsync(cancellationToken);
            _committed = true;
        }

        public async ValueTask DisposeAsync()
        {
            if (!_committed)
            {
                await _transaction.RollbackAsync();
            }

            await _transaction.DisposeAsync();
        }
    }
}

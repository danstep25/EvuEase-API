namespace EvuEase.Application.Interfaces.Persistence;




public interface IApplicationUnitOfWork
{
    Task<IApplicationDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}




public interface IApplicationDbTransaction : IAsyncDisposable
{
    Task CommitAsync(CancellationToken cancellationToken = default);
}

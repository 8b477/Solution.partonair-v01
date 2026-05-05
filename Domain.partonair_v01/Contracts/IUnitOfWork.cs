
namespace Domain.partonair_v01.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        // Repo
        IUserRepository Users { get; }
        IProfileRepository Profiles { get; }
        IContactRepository Contacts { get; }
        IEvaluationRepository Evaluations { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();

        Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation);
        Task ExecuteInTransactionAsync(Func<Task> operation);
    }
}

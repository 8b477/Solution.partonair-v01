using Domain.partonair_v01.Contracts;
using Domain.partonair_v01.Entities;
using Domain.partonair_v01.Exceptions;
using Domain.partonair_v01.Exceptions.Enums;
using Infrastructure.partonair_v01.ORM.EFCore.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.ComponentModel.DataAnnotations;


namespace Infrastructure.partonair_v01.Repositories
{
    public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
    {
        private readonly ApplicationDbContext _context = context;
        private IDbContextTransaction? _transaction;
        private bool _disposed;

        // I_REPO + LAZY LOAD
        private IUserRepository? _userRepository;
        public IUserRepository Users => _userRepository ??= new UserRepository(_context);

        private IProfileRepository? _profileRepository;
        public IProfileRepository Profiles => _profileRepository ??= new ProfileRepository(_context);

        private IContactRepository? _contactRepository;
        public IContactRepository Contacts => _contactRepository ??= new ContactRepository(_context);

        private IEvaluationRepository? _evaluationRepository;
        public IEvaluationRepository Evaluations => _evaluationRepository ??= new EvaluationRepository(_context);
        // ---

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InfrastructureLayerException(InfrastructureLayerErrorType.ConcurrencyDatabaseException);
            }
            catch (DbUpdateException)
            {
                throw new InfrastructureLayerException(InfrastructureLayerErrorType.UpdateDatabaseException);
            }
            catch (OperationCanceledException)
            {
                throw new InfrastructureLayerException(InfrastructureLayerErrorType.CancelationDatabaseException);
            }
            catch (Exception)
            {
                throw new InfrastructureLayerException(InfrastructureLayerErrorType.UnexpectedDatabaseException);
            }
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                if (_transaction is null)
                    throw new InfrastructureLayerException(InfrastructureLayerErrorType.NoActiveTransactionException);

                await _transaction.CommitAsync();             
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            try
            {
                if (_transaction is null)
                    throw new InfrastructureLayerException(InfrastructureLayerErrorType.NoActiveTransactionException);

                await _transaction.RollbackAsync();
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await BeginTransactionAsync();
                try
                {
                    var result = await operation();
                    await SaveChangesAsync();
                    await CommitTransactionAsync();
                    return result;
                }
                catch
                {
                    await RollbackTransactionAsync();
                    throw;
                }
            });
        }

        public async Task ExecuteInTransactionAsync(Func<Task> operation)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                await BeginTransactionAsync();
                try
                {
                    await operation();
                    await SaveChangesAsync();
                    await CommitTransactionAsync();
                }
                catch
                {
                    await RollbackTransactionAsync();
                    throw;
                }
            });
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _transaction?.Dispose();
                _context.Dispose();
            }
            _disposed = true;
        }
    }
}

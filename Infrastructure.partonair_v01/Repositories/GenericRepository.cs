using Domain.partonair_v01.Contracts;
using Domain.partonair_v01.Exceptions;
using Domain.partonair_v01.Exceptions.Enums;
using Infrastructure.partonair_v01.ORM.EFCore.Settings;

using Microsoft.EntityFrameworkCore;

namespace Infrastructure.partonair_v01.Repositories
{
    public abstract class GenericRepository<T> : IGenericRepository<T> where T : class, IEntity
    {
        protected readonly ApplicationDbContext _ctx;
        protected readonly DbSet<T> _dbSet;

        protected GenericRepository(ApplicationDbContext ctx)
        {
            _ctx = ctx 
                ?? throw new InfrastructureLayerException(InfrastructureLayerErrorType.DatabaseConnectionErrorException, $"- Context name : {nameof(_ctx)}");

            _dbSet = _ctx.Set<T>();
        }

        public virtual async Task<T> CreateAsync(T entity)
        {
            try
            {
                var result = await _dbSet.AddAsync(entity);

                return result.Entity;
            }
            catch (OperationCanceledException ex)
            {
                throw new InfrastructureLayerException(InfrastructureLayerErrorType.CancelationDatabaseException,$"{ex.Message}");
            }
        }

        public virtual async Task Delete(Guid id)
        {
            try
            {               
                var existingEntity = await GetByGuidAsync(id);

                bool isNotTracked = _ctx.Entry(existingEntity).State == EntityState.Detached;

                if (isNotTracked)
                    _ctx.Attach(existingEntity);

                _dbSet.Remove(existingEntity);
            }
            catch (OperationCanceledException ex)
            {
                throw new InfrastructureLayerException(InfrastructureLayerErrorType.CancelationDatabaseException, $"{ex.Message}");
            }
        }

        public virtual async Task<T> Update(T entity)
        {
            try
            {
                _ctx.Update(entity);

                _ctx.Entry(entity).State = EntityState.Modified;

                return entity;
            }
            catch (OperationCanceledException ex)
            {
                throw new InfrastructureLayerException(InfrastructureLayerErrorType.CancelationDatabaseException, $"{ex.Message}");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new InfrastructureLayerException(InfrastructureLayerErrorType.UpdateDatabaseException, $"{ex.Message}");
            }
        }

        public virtual async Task<ICollection<T>> GetAllAsync()
        {
            try
            {
                var entityList = await _dbSet.ToListAsync();
                return entityList;
            }
            catch (OperationCanceledException ex)
            {
                throw new InfrastructureLayerException(InfrastructureLayerErrorType.CancelationDatabaseException, $"{ex.Message}");
            }
        }

        public virtual async Task<T> GetByGuidAsync(Guid id)
        {
            try
            {
                var entity = await _dbSet.FindAsync(id);

                return
                    entity
                    ??
                    throw new InfrastructureLayerException(InfrastructureLayerErrorType.ResourceNotFoundException, $"Identifier : {id} - no match");
            }
            catch (OperationCanceledException ex)
            {
                throw new InfrastructureLayerException(InfrastructureLayerErrorType.CancelationDatabaseException, $"{ex.Message}");
            }
        }


    } 
}

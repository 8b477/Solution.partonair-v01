using Domain.partonair_v01.Contracts;
using Domain.partonair_v01.Entities;
using Domain.partonair_v01.Exceptions;
using Domain.partonair_v01.Exceptions.Enums;
using Infrastructure.partonair_v01.ORM.EFCore.Settings;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.partonair_v01.Repositories
{
    public class EvaluationRepository(ApplicationDbContext ctx) : GenericRepository<Evaluation>(ctx), IEvaluationRepository
    {
        public async override Task<ICollection<Evaluation>> GetAllAsync()
        {
            var result = await _dbSet
                                    .OrderBy(e => e.EvaluationCreatedAt)
                                    .Include(e =>e.Sender)
                                    .ToListAsync();
            return result ?? [];
        }

        public override async Task<Evaluation> GetByGuidAsync(Guid id)
        {
            var result = await _dbSet
                                    .Where(e => e.Id == id)
                                    .Include(e => e.Sender)
                                    .FirstOrDefaultAsync();

            return result ?? throw new InfrastructureLayerException(InfrastructureLayerErrorType.EntityIsNullException, $"Identifier : {id} - No match");
        }

    }
}

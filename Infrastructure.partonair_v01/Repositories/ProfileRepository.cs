

using Domain.partonair_v01.Contracts;
using Domain.partonair_v01.Entities;
using Infrastructure.partonair_v01.ORM.EFCore.Settings;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.partonair_v01.Repositories
{
    public class ProfileRepository : GenericRepository<ProfileUser>, IProfileRepository
    {
        public ProfileRepository(ApplicationDbContext ctx) : base(ctx)
        {
        }

        public async Task<ProfileUser?> GetPublicByGuidAsync(Guid id)
        {
            return await _dbSet
                .Include(profile => profile.User)
                .FirstOrDefaultAsync(profile => profile.Id == id && profile.IsPublic);
        }

        public async Task<ProfileUser?> GetWithUserByGuidAsync(Guid id)
        {
            return await _dbSet
                .Include(profile => profile.User)
                .FirstOrDefaultAsync(profile => profile.Id == id);
        }

        public async Task<ProfileUser?> GetByUserIdAsync(Guid userId)
        {
            return await _dbSet.FirstOrDefaultAsync(profile => profile.UserId == userId);
        }
    }
}

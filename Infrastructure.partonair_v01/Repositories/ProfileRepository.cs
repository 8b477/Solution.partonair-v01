

using Domain.partonair_v01.Contracts;
using Domain.partonair_v01.Entities;
using Infrastructure.partonair_v01.ORM.EFCore.Settings;

namespace Infrastructure.partonair_v01.Repositories
{
    public class ProfileRepository : GenericRepository<ProfileUser>, IProfileRepository
    {
        public ProfileRepository(ApplicationDbContext ctx) : base(ctx)
        {
        }

    }
}

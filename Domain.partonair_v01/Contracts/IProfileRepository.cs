using Domain.partonair_v01.Entities;


namespace Domain.partonair_v01.Contracts
{
    public interface IProfileRepository : IGenericRepository<ProfileUser>
    {
        Task<ProfileUser?> GetPublicByGuidAsync(Guid id);
        Task<ProfileUser?> GetByUserIdAsync(Guid userId);
        Task<ProfileUser?> GetWithUserByGuidAsync(Guid id);
    }
}

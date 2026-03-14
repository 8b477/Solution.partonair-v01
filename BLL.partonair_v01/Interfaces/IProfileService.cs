using SharedModels.partonair_v01.DTOS;

namespace BLL.partonair_v01.Interfaces
{
    public interface IProfileService
    {
        Task<ProfileViewDTO> CreateAsyncService(Guid idUser, ProfileCreateDTO entity);
        Task<ProfileViewDTO> UpdateAsyncService(Guid id, ProfileUpdateDTO entity);
        Task DeleteAsyncService(Guid id);
        Task<ProfileViewDTO> GetByGuidAsyncService(Guid id);
        Task<ICollection<ProfileAndUserViewDTO>> GetByRoleAsyncService(string role);
    }
}

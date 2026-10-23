using SharedModels.partonair_v01.DTOS;

namespace BLL.partonair_v01.Interfaces
{
    public interface IProfileService
    {
        Task<ProfileViewDTO> CreateAsyncService(Guid idUser, ProfileCreateDTO entity);
        Task<ProfileViewDTO> UpdateAsyncService(Guid idUser, Guid id, ProfileUpdateDTO entity);
        Task<MyProfileViewDTO> GetMyProfileAsyncService(Guid idUser);
        Task<MyProfileViewDTO> UpdateMyProfileAsyncService(Guid idUser, MyProfileUpdateDTO entity);
        Task<AvatarRegenerateResultDTO> RegenerateAvatarAsyncService(Guid idUser);
        Task DeleteAsyncService(Guid idUser, Guid idProfile);
        Task<ProfileViewDTO> GetByGuidAsyncService(Guid id);
        Task<PublicProfileViewDTO?> GetPublicByGuidAsyncService(Guid id, Guid? viewerId);
        Task<ICollection<PublicProfileViewDTO>> GetByRoleAsyncService(string role, Guid? currentUserId);
    }
}

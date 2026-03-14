using Domain.partonair_v01.Entities;
using SharedModels.partonair_v01.DTOS;


namespace BLL.partonair_v01.Interfaces
{
    public interface IUserService
    {
        // Commands
        Task<UserViewDTO> CreateAsyncService(UserCreateDTO entity);
        Task<UserViewDTO> UpdateService(Guid id, UserUpdateNameOrMailOrPasswordDTO entity);
        Task DeleteService(Guid id);

       // Task<bool> ChangeRoleAsyncService(Guid id, UserChangeRoleDTO role);

        // Queries
        Task<ICollection<UserViewDTO>> GetAllAsyncService();
        Task<UserViewDTO> GetByIdAsyncService(Guid id);
        Task<ICollection<UserViewDTO>> GetByNameAsyncService(string name);
        Task<UserViewDTO> GetByEmailAsyncService(string email);
        Task<ICollection<UserViewDTO>> GetByRoleAsyncService(string role);



        Task<User?> LogTest(string Mail, string Pass);
    }
}

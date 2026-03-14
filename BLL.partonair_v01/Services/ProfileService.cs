using BLL.partonair_v01.Interfaces;
using BLL.partonair_v01.Mappers;
using Domain.partonair_v01.Contracts;
using Domain.partonair_v01.Entities;
using Domain.partonair_v01.Enums;
using Domain.partonair_v01.Exceptions;
using Domain.partonair_v01.Exceptions.Enums;
using SharedModels.partonair_v01.DTOS;

namespace BLL.partonair_v01.Services
{
    public class ProfileService(IUnitOfWork UOW) : IProfileService
    {
        private readonly IUnitOfWork _UOW = UOW;

        public async Task<ProfileViewDTO> CreateAsyncService(Guid idUser, ProfileCreateDTO profileDTO)
        {
            try
            {
                await _UOW.BeginTransactionAsync();

                var existingUser = await _UOW.Users.GetByGuidAsync(idUser);

                if (existingUser.ProfileUser is not null)
                    throw new ApplicationLayerException(ApplicationLayerErrorType.ConstraintViolationErrorException,
                        "The user already has a profile, please use Update endpoint or Delete before creating a new profile.");

                ProfileUser profileToAdd = profileDTO.ToEntity(existingUser);
                var profilCreated = await _UOW.Profiles.CreateAsync(profileToAdd);

                existingUser.ProfileUser = profilCreated;

                var result = await _UOW.Users.Update(existingUser);

                await _UOW.SaveChangesAsync();
                await _UOW.CommitTransactionAsync();

                return profilCreated.ToView();
            }
            catch
            {
                await _UOW.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<ProfileViewDTO> GetByGuidAsyncService(Guid id)
        {
            var result = await _UOW.Profiles.GetByGuidAsync(id);

            return result.ToView();
        }

        public async Task<ICollection<ProfileAndUserViewDTO>> GetByRoleAsyncService(string role)
        {
            if (!Enum.TryParse<Roles>(role, true, out var validRole))
                throw new ApplicationLayerException(ApplicationLayerErrorType.ConstraintViolationErrorException, "The valid Role are : Visitor, Employee, Company - NO CASE SENSITIVE");

            var users = await _UOW.Users.GetByRoleIncludeProfilAsync(role);

            var result = users.Where(u => u.Role.ToString() == role) .ToList();

            ICollection<UserViewDTO> usersView = users.Select(u => u.ToView()).ToList();
            ICollection<ProfileViewDTO?> profileView = users.Select(u => u?.ProfileUser?.ToView()).ToList();

            ICollection<ProfileAndUserViewDTO> finalView = usersView
                .Zip(profileView, (user, profile) => new ProfileAndUserViewDTO(user, profile))
                .ToList();

            return finalView;
        }

        public async Task<ProfileViewDTO> UpdateAsyncService(Guid id, ProfileUpdateDTO entity)
        {
            var existingProfil = await _UOW.Profiles.GetByGuidAsync(id);

            entity.ToEntity(existingProfil);

            var result = await _UOW.Profiles.Update(existingProfil);

            await _UOW.SaveChangesAsync();

            return result.ToView();
        }

        public async Task DeleteAsyncService(Guid id)
        {
            try
            {
                await _UOW.BeginTransactionAsync();

                await _UOW.Profiles.Delete(id);

                var existingUser = await _UOW.Users.GetByForeignKeyProfilAsync(id);

                existingUser.ProfileUser = null;

                await _UOW.Users.Update(existingUser);

                await _UOW.SaveChangesAsync();
                await _UOW.CommitTransactionAsync();

                return;
            }
            catch
            {
                await _UOW.RollbackTransactionAsync();
                throw;
            }
        }

    }
}

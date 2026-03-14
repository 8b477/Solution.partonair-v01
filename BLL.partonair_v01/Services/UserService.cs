using BLL.partonair_v01.Interfaces;
using BLL.partonair_v01.Mappers;
using SharedModels.partonair_v01.DTOS;
using Domain.partonair_v01.Exceptions.Enums;
using Domain.partonair_v01.Contracts;
using Domain.partonair_v01.Exceptions;
using Domain.partonair_v01.Entities;
using Domain.partonair_v01.Enums;


namespace BLL.partonair_v01.Services
{
    public class UserService(IUnitOfWork UOW, IBCryptService bCryptService) : IUserService
    {
        private readonly IUnitOfWork _UOW = UOW;
        private readonly IBCryptService _bCryptService = bCryptService;

        #region Commands
        public async Task<UserViewDTO> CreateAsyncService(UserCreateDTO u)
        {
            bool isValidMail = await _UOW.Users.IsEmailAvailableAsync(u.Email);

            if (!isValidMail)
                throw new ApplicationLayerException(ApplicationLayerErrorType.ConstraintViolationErrorException);


            User newUser = u.ToEntity();

            newUser.PasswordHashed = _bCryptService.HashPass(newUser.PasswordHashed);


            var userAdded = await _UOW.Users.CreateAsync(newUser);

            await _UOW.SaveChangesAsync();


            return userAdded.ToView();
        }

        public async Task<UserViewDTO> UpdateService(Guid id, UserUpdateNameOrMailOrPasswordDTO entity)
        {
            var existingUser = await _UOW.Users.GetByGuidAsync(id);

            UpdatePassword(existingUser, entity);

            await UpdateEmail(existingUser, entity);

            entity.ToEntity(existingUser);

            var userUpdated = await _UOW.Users.Update(existingUser);

            await _UOW.SaveChangesAsync();

            return userUpdated.ToView();
        }


        public async Task DeleteService(Guid id)
        {
            await _UOW.Users.Delete(id);

            await _UOW.SaveChangesAsync();

            return;
        }

        #endregion



        #region Queries

        public async Task<UserViewDTO> GetByEmailAsyncService(string email)
        {
            var user = await _UOW.Users.GetByEmailAsync(email);

            return user.ToView();
        }

        public async Task<UserViewDTO> GetByIdAsyncService(Guid id)
        {
            var userEntity = await _UOW.Users.GetByGuidAsync(id);

            return userEntity.ToView();
        }

        public async Task<ICollection<UserViewDTO>> GetByNameAsyncService(string name)
        {
           var usersList = await _UOW.Users.GetByNameAsync(name);

           return usersList.Select(x => x.ToView())
                           .ToList();
        }

        public async Task<ICollection<UserViewDTO>> GetByRoleAsyncService(string role)
        {
            if(!Enum.TryParse<Roles>(role, true, out var validRole))
                throw new ApplicationLayerException(ApplicationLayerErrorType.ConstraintViolationErrorException, "The valid Role are : Visitor, Employee, Company - NO CASE SENSITIVE");

            var userList = await _UOW.Users.GetByRoleAsync(validRole.ToString());

            return userList.Select(x => x.ToView())
                           .ToList();
        }
    
        public async Task<ICollection<UserViewDTO>> GetAllAsyncService()
        {
            var result = await _UOW.Users.GetAllAsync();

            return result.Select(user => user.ToView())
                                                     .ToList();
        }

        #endregion



        #region Private Methods

        /// <summary>
        /// Compare stored hashed password with not hashed password, if it matches update with the new password supplied.
        /// </summary>
        /// <param name="userData">The entity containing the hashed password</param>
        /// <param name="newUserData">The entity containing the actual no hashed password and the new to add</param>
        private void UpdatePassword(User userData, UserUpdateNameOrMailOrPasswordDTO newUserData)
        {
            if (IsValidNewPassword(newUserData)) 
            {
                bool IsValidateByBCryptService = _bCryptService.VerifyPasswordMatch(newUserData.OldPassword, userData.PasswordHashed);

                if (IsValidateByBCryptService)
                {
                    string newPassHashed = _bCryptService.HashPass(newUserData.NewPassword);

                    newUserData.NewPassword = newPassHashed;

                    return;
                }
            }  
            return;
        }

        /// <summary>
        /// Check if properties : <c>NewPassword</c> and <c>OldPassword</c> is null or empty.
        /// </summary>
        /// <param name="newUserData">The entity to verify properties</param>
        /// <returns>Returns <c>True</c> if properties are not empty and not null; otherwise <c>False</c></returns>
        private static bool IsValidNewPassword(UserUpdateNameOrMailOrPasswordDTO newUserData)
        {
            bool IsValidNewPass = !string.IsNullOrWhiteSpace(newUserData.NewPassword);
            bool IsValidOldPass = !string.IsNullOrWhiteSpace(newUserData.OldPassword);

            if (IsValidNewPass && IsValidOldPass)
                return true;

            return false;
        }

        /// <summary>
        /// Check that the mail is non-null and non-empty and respects the Unique database constraint 
        /// </summary>
        /// <param name="userData">The entity containing the stored data</param>
        /// <param name="newUserData">The entity containing the new mail to add</param>
        /// <exception cref="ApplicationLayerException"></exception>
        private async Task UpdateEmail(User userData, UserUpdateNameOrMailOrPasswordDTO newUserData)
        {
            if (!string.IsNullOrWhiteSpace(newUserData.Email))
            {
                if (await _UOW.Users.IsEmailAvailableAsync(newUserData.Email))
                {
                    userData.Mail = newUserData.Email;
                    return;
                }

                throw new ApplicationLayerException(ApplicationLayerErrorType.ConstraintViolationErrorException, $"Mail supplied : {newUserData.Email}");
            }
            return;
        }

        #endregion

        public async Task<User?> LogTest(string Mail, string Pass)
        {
            var existingUser = await _UOW.Users.GetByEmailAsync(Mail);

            bool IsValidateByBCryptService = _bCryptService.VerifyPasswordMatch(Pass, existingUser.PasswordHashed);

            if (IsValidateByBCryptService)
            {
                return existingUser;
            }
            return null;
        }



    }
}
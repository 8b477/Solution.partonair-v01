using Domain.partonair_v01.Entities;
using Domain.partonair_v01.Enums;
using SharedModels.partonair_v01.DTOS;

namespace BLL.partonair_v01.Mappers
{
    internal static class UserMapper
    {
        internal static User ToEntity(this UserCreateDTO u)
        {
            return new User
            {
                Id = Guid.NewGuid(),
                Mail = u.Email,
                Name = u.UserName,
                PasswordHashed = u.Password,
                LastConnection = DateTime.Now,
                UserCreatedAt = DateTime.Now,
                Role = Roles.Visitor,
                IsActive = true,
                ProfileUser = null,
            };
        }

        internal static UserViewDTO ToView(this User u)
        {
            return new UserViewDTO
                (
                    u.Id,
                    u.Name,
                    u.Mail,
                    u.IsActive,
                    u.UserCreatedAt,
                    u.LastConnection,
                    u.Role.ToString(),
                    u.ProfileUser?.Id
                );
        }


        internal static void ToEntity(this UserUpdateNameOrMailOrPasswordDTO u, User e)
        {
            e.Mail = u.Email ?? e.Mail;
            e.Name = u.UserName ?? e.Name;
            e.PasswordHashed = u.NewPassword ?? e.PasswordHashed;         
        }

    }
}

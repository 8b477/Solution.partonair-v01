using Domain.partonair_v01.Contracts;
using Domain.partonair_v01.Entities;
using Domain.partonair_v01.Exceptions;
using Domain.partonair_v01.Exceptions.Enums;
using Domain.partonair_v01.Enums;
using Infrastructure.partonair_v01.ORM.EFCore.Settings;
using Microsoft.EntityFrameworkCore;
using System.Data;


namespace Infrastructure.partonair_v01.Repositories
{
    public class UserRepository(ApplicationDbContext ctx) : GenericRepository<User>(ctx), IUserRepository
    {

        #region <-------------> GET <------------->

        public async Task<User> GetByEmailAsync(string email)
        {
            var result = await _dbSet.Where(u  => u.Mail == email)
                                         .FirstOrDefaultAsync();

            return 
                result
                ??
                throw new InfrastructureLayerException(InfrastructureLayerErrorType.ResourceNotFoundException, $"The mail : {email} - no match");
        }

        public async Task<ICollection<User>> GetByNameAsync(string name)
        {
            var result = await _dbSet.Where(u => u.Name == name)
                                         .ToListAsync();

            if(result.Count == 0)
                throw new InfrastructureLayerException(InfrastructureLayerErrorType.ResourceNotFoundException, $"The name : {name} - no match");

            return result;
        }

        public async Task<ICollection<User>> GetByRoleAsync(string role)
        {
            var result = await _dbSet.Where(u => u.Role.ToString() == role)
                                         .ToListAsync();

            if (result.Count == 0)
                throw new InfrastructureLayerException(InfrastructureLayerErrorType.ResourceNotFoundException, $"The role : {role} - no match");

            return result;
        }

        public async Task<ICollection<User>> GetByRoleIncludeProfilAsync(string role)
        {
            var result = await _dbSet
                                     .Where(u => u.Role.ToString() == role)
                                     .Include(u => u.ProfileUser)
                                     .ToListAsync();

            if (result.Count == 0)
                throw new InfrastructureLayerException(InfrastructureLayerErrorType.ResourceNotFoundException, $"The role : {role} - no match");

            return result;
        }

        public async Task<User> GetByForeignKeyProfilAsync(Guid idProfile)
        {
            var result = await _dbSet
                                     .Where(u => u.Id == idProfile)
                                     .FirstOrDefaultAsync();

            if (result is null)
                throw new InfrastructureLayerException(InfrastructureLayerErrorType.ResourceNotFoundException, $"The identifier : {idProfile} - no match");

            return result;
        }

        #endregion



        #region CHECK

        public async Task<bool> IsEmailAvailableAsync(string email)
        {
            var result = await _dbSet.Where(u => u.Mail == email)
                                         .AnyAsync();

            return !result ;
        }


        public async Task<bool> IsUserWithoutProfil(Guid id)
        {
            var existingUser = await _dbSet.FirstOrDefaultAsync(u => u.Id == id)
                ?? throw new InfrastructureLayerException(InfrastructureLayerErrorType.EntityIsNullException, $"Identifier : {id} - No match");

            return 
                existingUser.ProfileUser is null
                ? true
                : false; 
        }

        #endregion



        #region Update

        public async Task<bool> ChangeRoleAsync(Guid id, string role)
        {
            var existingUser = await _dbSet.FindAsync(id)
                ?? throw new InfrastructureLayerException(InfrastructureLayerErrorType.EntityIsNullException, $"Identifier : {id} - No match");

            bool validRole = Enum.TryParse<Roles>(role,true,out var roleToAdd);

            if (!validRole)
                return false;

            existingUser.Role = roleToAdd;

            return true;
        }

        #endregion

    }
}

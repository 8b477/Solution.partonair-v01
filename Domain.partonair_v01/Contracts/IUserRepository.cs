using Domain.partonair_v01.Entities;


namespace Domain.partonair_v01.Contracts
{
    public interface IUserRepository : IGenericRepository<User>
    {
        /// <summary>
        /// Fetch a list of User by Name. 
        /// </summary>
        /// <param name="name">Represent the filter to apply</param>
        /// <returns>Returns an asynchronous task collection of User filtered by name</returns>
        Task<ICollection<User>> GetByNameAsync(string name);

        /// <summary>
        /// Fetch one User by Email.
        /// </summary>
        /// <param name="email">The email searched</param>
        /// <returns>Returns an asynchronous task with the matched User</returns>
        Task<User> GetByEmailAsync(string email);

        /// <summary>
        /// Fetch Users by an enumerator Roles.
        /// </summary>
        /// <param name="role">The role searched</param>
        /// <returns>Returns an asynchronous task list of User wich matched by Role</returns>
        Task<ICollection<User>> GetByRoleAsync(string role);

        /// <summary>
        /// Retrieve users and their profiles using a Roles enumerator..
        /// </summary>
        /// <param name="role">The role searched</param>
        /// <returns>Returns a list of asynchronous tasks for users whose profile matches the role.</returns>
        Task<ICollection<User>> GetByRoleIncludeProfilAsync(string role);

        /// <summary>
        /// Fetch a User by Foreign Key profile. 
        /// </summary>
        /// <param name="idProfile">Represent the identifier to find</param>
        /// <returns>Returns an asynchronous Entity <c>User</c> based on Foreign key Profile</returns>
        Task<User> GetByForeignKeyProfilAsync(Guid idProfile);

        /// <summary>
        /// Checks if the specified email already exists in the database.
        /// </summary>
        /// <param name="email">The email address to check.</param>
        /// <returns>
        /// <c>true</c> if the email does not exist in the database; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> IsEmailAvailableAsync(string email);

        /// <summary>
        /// Modify the User role.
        /// </summary>
        /// <param name="id">User identifier <c>'Guid'</c></param>
        /// <param name="role">The new role <c>'String'</c></param>
        /// <returns>Returns <c>True</c> if update is successful</returns>
        /// <exception cref="InfrastructureLayerException">Thrown when the user with the specified ID is not found</exception>
        Task<bool> ChangeRoleAsync(Guid id, string role);

        /// <summary>
        /// Check if the specified user has a profile.
        /// </summary>
        /// <param name="id">User identifier <c>'Guid'</c></param>
        /// <returns>Returns <c>True</c> if the user has no associated profile; otherwise <c>False</c>.</returns>
        /// <exception cref="InfrastructureLayerException">Thrown when the user with the specified ID is not found</exception>
        Task<bool> IsUserWithoutProfil(Guid id);

    }
}

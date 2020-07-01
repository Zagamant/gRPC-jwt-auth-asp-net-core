using System.Collections.Generic;
using System.Threading.Tasks;
using Server.DAL.Models;
using Server.Services.Models.UserManagement;

namespace Server.Services.UserManagement
{
    /// <summary>
    /// Represent userParam management service.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Authenticate userParam on server and create JWT-token async.
        /// </summary>
        /// <param name="userSignIn">User model with username and password for signing in</param>
        /// <returns>A <see cref="User"/></returns>
        Task<User> AuthenticateAsync(UserSignIn userSignIn);

        /// <summary>
        /// Get all users from database async.
        /// </summary>
        /// <returns>List of <see cref="User">Users</see></returns>
        Task<List<User>> GetAllAsync();

        /// <summary>
        /// Get one userParam from database by id async.
        /// </summary>
        /// <param name="id">A userParam identifier.</param>
        /// <returns></returns>
        Task<User> GetByIdAsync(int id);

        /// <summary>
        /// Create new userParam and return it back async.
        /// </summary>
        /// <param name="userParam">A <see cref="UserSignUp"/>.</param>
        /// <returns>A <see cref="User"/></returns>
        Task<User> CreateAsync(UserSignUp userParam);

        /// <summary>
        /// Update existed userParam async.
        /// </summary>
        /// <param name="user">A <see cref="UserUpdate"/>.</param>
        Task UpdateAsync(UserUpdate user);

        /// <summary>
        /// Delete existed userParam by id async.
        /// </summary>
        /// <param name="id">A userParam identifier.</param>
        Task DeleteAsync(int id);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Server.DAL;
using Server.DAL.Models;
using Server.Services.Models.UserManagement;

namespace Server.Services.UserManagement
{
    /// <summary>
    /// Represent a userParam service
    /// </summary>
    public class UserService : IUserService
    {
        private readonly UserDbContext _context;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initialize a new instance of the <see cref="UserService" /> class with specified <see cref="UserDbContext" />.
        /// </summary>
        /// <param name="context">A <see cref="UserDbContext" />Database instance</param>
        /// <param name="mapper">A <see cref="Mapper" /></param>
        public UserService(UserDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <inheritdoc />
        public async Task<User> AuthenticateAsync(UserSignIn userSignIn)
        {
            if(string.IsNullOrEmpty(userSignIn.UserName) || string.IsNullOrEmpty(userSignIn.Password))
                return null;

            var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == userSignIn.UserName);

            if(user == null)
                return null;

            return !VerifyPasswordHash(userSignIn.Password, Convert.FromBase64String(user.PasswordHash), user.PasswordSalt)
                ? null
                : user;
        }

        /// <inheritdoc />
        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<User> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        /// <inheritdoc />
        public async Task<User> CreateAsync(UserSignUp userParam)
        {
            var user = _mapper.Map<User>(userParam);

            if(string.IsNullOrWhiteSpace(userParam.Password))
                throw new ArgumentException("Password Hash is required");

            if(_context.Users.Any(x => x.UserName == userParam.UserName))
                throw new ArgumentException("UserName \"" + userParam.UserName + "\" is already taken");

            CreatePasswordHash(userParam.Password, out var passwordHash, out var passwordSalt);

            user.PasswordHash = Convert.ToBase64String(passwordHash);
            user.PasswordSalt = passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        /// <inheritdoc />
        public async Task UpdateAsync(UserUpdate userParam)
        {
            var user = await _context.Users.FindAsync(userParam.Id);

            if(user == null)
                throw new ArgumentException("User not found");

            if(!string.IsNullOrWhiteSpace(userParam.UserName) && userParam.UserName != user.UserName)
            {
                if(await _context.Users.AnyAsync(x => x.UserName == userParam.UserName).ConfigureAwait(true))
                    throw new ArgumentException("UserName " + userParam.UserName + " is already taken");

                user.UserName = userParam.UserName;
            }

            if(!string.IsNullOrWhiteSpace(userParam.Email) && userParam.UserName != user.Email)
            {
                if(await _context.Users.AnyAsync(x => x.Email == userParam.Email).ConfigureAwait(true))
                    throw new ArgumentException("Email " + userParam.Email + " is already taken");

                user.Email = userParam.Email;
            }

            if(!string.IsNullOrWhiteSpace(userParam.FirstName))
                user.FirstName = userParam.FirstName;

            if(!string.IsNullOrWhiteSpace(userParam.LastName))
                user.LastName = userParam.LastName;

            if(!string.IsNullOrWhiteSpace(userParam.PasswordNew))
            {
                if(VerifyPasswordHash(userParam.PasswordOld, Convert.FromBase64String(user.PasswordHash), user.PasswordSalt))
                    throw new ArgumentException("Password is wrong");

                CreatePasswordHash(userParam.PasswordNew, out var passwordHash, out var passwordSalt);

                user.PasswordHash = Convert.ToBase64String(passwordHash);
                user.PasswordSalt = passwordSalt;
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if(user == null)
                return;
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        #region private helper methods

        /// <summary>
        /// Create password hash and uniq salt for new userParam.
        /// </summary>
        /// <param name="password">Clear password.</param>
        /// <param name="passwordHash">Encrypted password.</param>
        /// <param name="passwordSalt">PasswordHash's salt.</param>
        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if(password == null)
                throw new ArgumentNullException(nameof(password));
            if(string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(password));

            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        /// <summary>
        /// Verify is entered password equals hashed ones.
        /// </summary>
        /// <param name="password">Entered password.</param>
        /// <param name="storedHash">Hashed password.</param>
        /// <param name="storedSalt">Uniq salt that were used to encrypt password.</param>
        /// <returns>Is it corrected entered password.</returns>
        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if(password == null)
                throw new ArgumentNullException(nameof(password));
            if(string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(password));
            if(storedHash.Length != 64)
                throw new ArgumentException("Invalid length of password hash (64 bytes expected).", nameof(storedHash));
            if(storedSalt.Length != 128)
                throw new ArgumentException("Invalid length of password salt (128 bytes expected).",
                    nameof(storedSalt));

            using var hmac = new HMACSHA512(storedSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return !computedHash.Where((t, i) => t != storedHash[i]).Any();
        }

        #endregion
    }
}
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Server.DAL;

namespace Server.Services.UserManagement
{
    /// <summary>
    /// Represent a userParam service
    /// </summary>
    public class UserService : Server.UserService.UserServiceBase
    {
        private readonly UserDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;


        /// <summary>
        /// Initialize a new instance of the <see cref="UserService" /> class with specified <see cref="UserDbContext" />.
        /// </summary>
        /// <param name="context">A <see cref="UserDbContext" />Database instance</param>
        /// <param name="mapper">A <see cref="Mapper" /></param>
        /// <param name="logger">A <see cref="Logger" /></param>
        public UserService(UserDbContext context, IMapper mapper, ILogger<UserService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override async Task<User> Authenticate(UserSignInRequest request, ServerCallContext context)
        {
            if(string.IsNullOrEmpty(request.User.UserName) || string.IsNullOrEmpty(request.User.Password))
                return null;

            var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == request.User.UserName);
            var userResponse = _mapper.Map<User>(user);
            if(user == null)
                return null;

            return !VerifyPasswordHash(request.User.Password, Convert.FromBase64String(user.PasswordHash), user.PasswordSalt)
                ? null
                : userResponse;
        }

        /// <inheritdoc />
        public override async Task GetAll(Void request, IServerStreamWriter<User> responseStream, ServerCallContext context)
        {
            var users = await _context.Users.ToListAsync();
            foreach(var user in users)
            {
                await responseStream.WriteAsync(_mapper.Map<User>(user));
            }
        }

        /// <inheritdoc />
        public override async Task<User> GetById(UserIdRequest request, ServerCallContext context)
        {
            var user = await _context.Users.FindAsync(request.Id);
            var userResponse = _mapper.Map<User>(user);
            return userResponse;
        }

        /// <inheritdoc />
        public override async Task<User> Create(UserSignUpRequest request, ServerCallContext context)
        {
            var user = _mapper.Map<User>(request);

            if(string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Password Hash is required");

            if(_context.Users.Any(x => x.UserName == request.UserName))
                throw new ArgumentException("UserName \"" + request.UserName + "\" is already taken");

            CreatePasswordHash(request.Password, out var passwordHash, out var passwordSalt);

            user.PasswordHash = Convert.ToBase64String(passwordHash);
            user.PasswordSalt = ByteString.CopyFrom(passwordSalt);

            var originUser = _mapper.Map<DAL.Models.User>(user);

            await _context.Users.AddAsync(originUser);
            await _context.SaveChangesAsync();

            return user;
        }

        /// <inheritdoc />
        public override async Task<Void> Update(UserUpdateRequest request, ServerCallContext context)
        {
            var user = await _context.Users.FindAsync(request.Id);

            if(user == null)
                throw new ArgumentException("User not found");

            if(!string.IsNullOrWhiteSpace(request.UserName) && request.UserName != user.UserName)
            {
                if(await _context.Users.AnyAsync(x => x.UserName == request.UserName).ConfigureAwait(true))
                    throw new ArgumentException("UserName " + request.UserName + " is already taken");

                user.UserName = request.UserName;
            }

            if(!string.IsNullOrWhiteSpace(request.Email) && request.UserName != user.Email)
            {
                if(await _context.Users.AnyAsync(x => x.Email == request.Email).ConfigureAwait(true))
                    throw new ArgumentException("Email " + request.Email + " is already taken");

                user.Email = request.Email;
            }

            if(!string.IsNullOrWhiteSpace(request.FirstName))
                user.FirstName = request.FirstName;

            if(!string.IsNullOrWhiteSpace(request.LastName))
                user.LastName = request.LastName;

            if(!string.IsNullOrWhiteSpace(request.PasswordNew))
            {
                if(VerifyPasswordHash(request.PasswordOld, Convert.FromBase64String(user.PasswordHash), user.PasswordSalt))
                    throw new ArgumentException("Password is wrong");

                CreatePasswordHash(request.PasswordNew, out var passwordHash, out var passwordSalt);

                user.PasswordHash = Convert.ToBase64String(passwordHash);
                user.PasswordSalt = passwordSalt;
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return new Void();
        }

        /// <inheritdoc />
        public override async Task<Void> Delete(UserIdRequest request, ServerCallContext context)
        {
            var user = await _context.Users.FindAsync(request.Id);
            if(user == null)
                throw new ArgumentException("User with id: " + request.Id + " not found.");
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return new Void();
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
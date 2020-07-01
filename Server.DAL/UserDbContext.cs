using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Server.DAL.Models;

namespace Server.DAL
{
    public class UserDbContext : IdentityDbContext<User, Role, int>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserDbContext"/> class.
        /// </summary>
        public UserDbContext(DbContextOptions options) : base(options) { }

    }
}

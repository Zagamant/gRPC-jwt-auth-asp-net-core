using System;
using Microsoft.AspNetCore.Identity;

namespace Server.DAL.Models
{
    public class User : IdentityUser<int>
    {
        /// <summary>
        /// Gets or sets a user's role.
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// Gets or sets a user's firstname.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets a user's lastname.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets a user's password salt.
        /// </summary>
        public byte[] PasswordSalt { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Server.Services.Models.UserManagement
{
    /// <summary>
    /// Represent authentication model for client update information
    /// </summary>
    public class UserUpdate
    {
        /// <summary>
        /// Gets or sets user's Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets user's firstname
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets user's lastname
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets user's username
        /// </summary>
        public string UserName { get; set; }


        /// <summary>
        /// Gets or sets user's password
        /// </summary>
        public string PasswordOld { get; set; }

        /// <summary>
        /// Gets or sets user's password
        /// </summary>
        public string PasswordNew { get; set; }

        /// <summary>
        /// Gets or sets user's email
        /// </summary>
        public string Email { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Server.Services.Models.UserManagement
{
    /// <summary>
    /// Represent authentication model for client authentication
    /// </summary>
    public class UserSignIn
    {
        /// <summary>
        /// Gets or sets user's email
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets user's password
        /// </summary>
        [Required]
        public string Password { get; set; }


    }
}
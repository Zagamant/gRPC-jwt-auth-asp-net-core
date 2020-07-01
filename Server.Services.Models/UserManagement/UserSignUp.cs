using System.ComponentModel.DataAnnotations;

namespace Server.Services.Models.UserManagement
{
    /// <summary>
    /// Represent authentication model for client registration
    /// </summary>
    public class UserSignUp
    {
        /// <summary>
        /// Gets or sets a user's firstname.
        /// </summary>
        [Required]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets a user's lastname.
        /// </summary>
        [Required]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets user's email
        /// </summary>
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets username
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

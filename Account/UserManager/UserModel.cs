
using System.ComponentModel.DataAnnotations;

namespace Authorization_Authentication.Account.UserManager
{
    public class UserModel
    {

        public int Id { get; set; }


        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(100, ErrorMessage = "Username must be between 3 and 100 characters", MinimumLength = 3)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, ErrorMessage = "Password must be at least 6 characters long", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; }

        public bool isloggin { get; set; } = false;

        [Required(ErrorMessage = "Password is Invalid")]
        public string CurrentPassword { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HotelsCommons.Models
{
    public class User : Common
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string HashedPassword { get; set; }
        public string Salt { get; set; }
        public string PasswordBackdoor { get; set; } // For demo purpose
        public ICollection<UserRole> UserRoles { get; set; }
        public ICollection<Hotel> Hotels { get; set; }
        public ICollection<UserHotel> UserHotels { get; set; }
        public ICollection<Booking> Bookings { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }

    // UserDTO for lighter retrieval of user info
    public class UserDTO
    {
        public string ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }

    public class UserCreateDTO
    {
        [Required(ErrorMessage = "First name is required.")]
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression("^[a-zA-Z0-9_.-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$", ErrorMessage = "Must be a valid email")]
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(64, ErrorMessage = "Password must be at least 8 characters.", MinimumLength = 8)]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[\\W_]).{8,}$", ErrorMessage = "Must contain at least one upper case letter, one lower case letter, one special character and one number.")]
        [JsonPropertyName("password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [JsonPropertyName("passwordConfirm")]
        public string PasswordConfirm { get; set; }
    }

    public class UserUpdateDTO
    {
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }
    }

    public class UserLoginDTO
    {
        [Required(ErrorMessage = "Email is required.")]
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }

    public class PasswordDTO
    {
        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("passwordConfirm")]
        public string PasswordConfirm { get; set; }
    }


    public class LoginResultDTO
    {
        public bool Successful { get; set; }
        public string Error { get; set; }

        public string Token { get; set; }

        public double ExpiresIn { get; set; }
        public string RefreshToken { get; set; }
    }

    public class UserResultDTO
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int RoleHierarchy { get; set; }
        public List<string> Roles { get; set; }

    }
}

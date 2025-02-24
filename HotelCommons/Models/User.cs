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
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

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
        [JsonPropertyName("email")]
        public string Email { get; set; }

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


    public class LoginResult
    {
        public bool Successful { get; set; }
        public string Error { get; set; }

        public string Token { get; set; }

        public double ExpiresIn { get; set; }
        public string RefreshToken { get; set; }
    }

    public class RegisterResult
    {
        public bool Successful { get; set; }
        public IEnumerable<string> Errors { get; set; }
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

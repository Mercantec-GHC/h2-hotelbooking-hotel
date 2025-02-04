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
        public List<Role> Roles { get; set; }
        public List<Hotel> Hotels { get; set; }
        public List<Booking> Bookings { get; set; }
    }

    // UserDTO for lighter retrieval of user info
    public class UserDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
    // UserCreateDTO to send relevant information when creating user
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
    }
    // UserLoginDTO to send relevant info to login in with user
    public class UserLoginDTO
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("passsword")]
        public string Password { get; set; }
    }
}

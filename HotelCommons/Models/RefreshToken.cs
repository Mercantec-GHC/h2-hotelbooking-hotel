using System.Text.Json.Serialization;

namespace HotelsCommons.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; }
    }

    public class RefreshTokenDTO
    {
        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; }
    }
}

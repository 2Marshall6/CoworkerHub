using System.Text.Json.Serialization;

namespace CoworkerHub.Application.DTOs.Authentication
{
    public class AuthenticationDTO
    {
        public string UserName { get; set; }
        public string AccessToken { get; set; }
        public int ExpiresIn { get; set; }
        [JsonIgnore]
        public string RefreshToken { get; set; }
    }
}

namespace CoworkerHub.Application.DTOs.Authentication
{
    public class TokenOptions
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Key { get; set; }
        public int TokenLifetimeInMinutes { get; set; } = 10;
        public int RefreshTokenLifetimeInDays { get; set; } = 14;  
    }
}

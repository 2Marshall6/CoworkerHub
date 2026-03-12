namespace CoworkerHub.Domain.Entities
{
    public class JwtOptions
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Key { get; set; }
        public int TokenLifetimeInMinutes { get; set; } = 10;
    }
}

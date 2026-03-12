using Microsoft.AspNetCore.Identity;

namespace CoworkerHub.Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        public Guid Id { get; set; }
    }
}

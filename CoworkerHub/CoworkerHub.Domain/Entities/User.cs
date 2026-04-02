using Microsoft.AspNetCore.Identity;

namespace CoworkerHub.Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}


using CoworkerHub.Domain.Enums;

namespace CoworkerHub.Domain.Entities
{
    public class Desk
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Floor { get; set; }
        public int Capacity { get; set; }
        public decimal PricePerHour { get; set; }

        public DeskStatus Status { get; set; }

        public int WorkspaceId { get; set; }
        public Workspace Workspace { get; set; }

        public ICollection<Booking> Booking { get; set; }
    }
}

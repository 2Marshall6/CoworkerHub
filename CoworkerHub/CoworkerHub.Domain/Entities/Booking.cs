using CoworkerHub.Domain.Enums;

namespace CoworkerHub.Domain.Entities
{
    public class Booking
    {
        public int Id { get; set; } 
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal TotalPrice { get; set; }

        public BookingStatus Status { get; set; }

        public int DeskId { get; set; }
        public Desk Desk { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}

using CoworkerHub.Domain.Enums;

namespace CoworkerHub.Application.DTOs.Booking
{
    public class BookingDTO
    {
        public int Id { get; set; }
        public int DeskId { get; set; }
        public Guid UserId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal TotalPrice { get; set; }
        public BookingStatus Status { get; set; }
    }
}
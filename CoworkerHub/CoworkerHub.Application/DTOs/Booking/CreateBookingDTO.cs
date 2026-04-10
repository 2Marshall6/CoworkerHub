namespace CoworkerHub.Application.DTOs.Booking
{
    public class CreateBookingDTO
    {
        public int DeskId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
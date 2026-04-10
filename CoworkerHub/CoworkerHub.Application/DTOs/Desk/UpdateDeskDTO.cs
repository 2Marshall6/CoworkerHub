namespace CoworkerHub.Application.DTOs.Desk
{
    public class UpdateDeskDTO
    {
        public string Name { get; set; } = string.Empty;
        public int Floor { get; set; }
        public int Capacity { get; set; }
        public decimal PricePerHour { get; set; }
    }
}

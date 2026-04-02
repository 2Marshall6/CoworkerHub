using CoworkerHub.Domain.Enums;

namespace CoworkerHub.Application.DTOs.Desk
{
    public class DeskDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Floor { get; set; }
        public int Capacity { get; set; }
        public decimal PricePerHour { get; set; }
        public DeskStatus Status { get; set; }
        public int WorkspaceId { get; set; }
    }
}

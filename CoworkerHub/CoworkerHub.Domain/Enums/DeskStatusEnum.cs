
namespace CoworkerHub.Domain.Enums
{
    public enum DeskStatus
    {
        Available = 0,      // Свободен
        Occupied = 1,       // Занят (физически)
        NeedsCleaning = 2,  // Требует уборки
        Maintenance = 3     // На ремонте / Недоступен
    }
}

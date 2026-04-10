namespace CoworkerHub.Domain.Enums
{
    public enum BookingStatus
    {
        Pending = 0,    // Ожидает подтверждения/оплаты
        Confirmed = 1,  // Подтверждена
        Cancelled = 2,  // Отменена
        Completed = 3   // Завершена (время вышло)
    }
}
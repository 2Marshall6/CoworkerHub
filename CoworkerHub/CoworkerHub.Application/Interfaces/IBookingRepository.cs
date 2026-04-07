using CoworkerHub.Domain.Entities;

namespace CoworkerHub.Application.Interfaces
{
    public interface IBookingRepository
    {
        void AddBooking(Booking booking);
        Task<bool> HasOverlappingBookingsAsync(int deskId, DateTime start, DateTime end, CancellationToken cancellationToken);
        Task<List<Booking>> GetBookingsByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    }
}
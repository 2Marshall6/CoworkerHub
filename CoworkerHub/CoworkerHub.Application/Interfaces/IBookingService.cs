using CoworkerHub.Application.DTOs.Booking;

namespace CoworkerHub.Application.Interfaces
{
    public interface IBookingService
    {
        Task<BookingDTO> CreateBookingAsync(Guid userId, CreateBookingDTO createModel, CancellationToken cancellationToken);

        Task<List<BookingDTO>> GetMyBookingsAsync(Guid userId, CancellationToken cancellationToken);
    }
}
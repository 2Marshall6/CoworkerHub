using CoworkerHub.Application.Interfaces;
using CoworkerHub.Domain.Entities;
using CoworkerHub.Domain.Enums;
using CoworkerHub.Infrastructure.Persistens;
using Microsoft.EntityFrameworkCore;

namespace CoworkerHub.Infrastructure.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AddBooking(Booking booking)
        {
            _context.Bookings.Add(booking);
        }

        public async Task<List<Booking>> GetBookingsByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _context.Bookings
                .AsNoTracking()
                .Where(b => b.UserId == userId)
                .Include(b => b.Desk) 
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> HasOverlappingBookingsAsync(int deskId, DateTime start, DateTime end, CancellationToken cancellationToken)
        {
            return await _context.Bookings.AnyAsync(b =>
                b.DeskId == deskId &&
                b.Status != BookingStatus.Cancelled &&
                b.StartTime < end &&
                b.EndTime > start,
                cancellationToken);
        }
    }
}
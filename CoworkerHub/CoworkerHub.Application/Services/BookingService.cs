using AutoMapper;
using CoworkerHub.Application.DTOs.Booking;
using CoworkerHub.Application.Exceptions;
using CoworkerHub.Application.Interfaces;
using CoworkerHub.Domain.Entities;
using CoworkerHub.Domain.Enums;

namespace CoworkerHub.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IDeskRepository _deskRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookingService(IBookingRepository bookingRepository, IDeskRepository deskRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _deskRepository = deskRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BookingDTO> CreateBookingAsync(Guid userId, CreateBookingDTO createModel, CancellationToken cancellationToken)
        {
            var desk = await _deskRepository.GetDeskByIdAsync(createModel.DeskId, cancellationToken);
            if (desk == null)
                throw new NotFoundException($"Desk with ID {createModel.DeskId} not found.");

            if (desk.Status == DeskStatus.Maintenance)
                throw new AppValidationException("This table is under repair and temporarily unavailable for booking.");

            bool isOccupied = await _bookingRepository.HasOverlappingBookingsAsync(createModel.DeskId, createModel.StartTime, createModel.EndTime, cancellationToken);
            if (isOccupied)
            {
                throw new AlreadyExistsException("Sorry, this table is already booked at the selected time.");
            }

            var durationInHours = (decimal)(createModel.EndTime - createModel.StartTime).TotalHours;

            var totalPrice = Math.Round(durationInHours * desk.PricePerHour, 2);

            var booking = new Booking
            {
                DeskId = createModel.DeskId,
                UserId = userId,
                StartTime = createModel.StartTime,
                EndTime = createModel.EndTime,
                TotalPrice = totalPrice,
                Status = BookingStatus.Pending 
            };


            _bookingRepository.AddBooking(booking);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<BookingDTO>(booking);
        }

        public async Task<List<BookingDTO>> GetMyBookingsAsync(Guid userId, CancellationToken cancellationToken)
        {
            var bookings = await _bookingRepository.GetBookingsByUserIdAsync(userId, cancellationToken);
            return _mapper.Map<List<BookingDTO>>(bookings);
        }
    }
}
using CoworkerHub.Application.DTOs.Booking;
using CoworkerHub.Application.Exceptions;
using CoworkerHub.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CoworkerHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IValidator<CreateBookingDTO> _validator;

        public BookingsController(IBookingService bookingService, IValidator<CreateBookingDTO> validator)
        {
            _bookingService = bookingService;
            _validator = validator;
        }

        [HttpPost]
        public async Task<ActionResult<BookingDTO>> Create(CreateBookingDTO createModel, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(createModel, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new AppValidationException(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
            }
             
            var userId = await FindUserIdAsync();

            var booking = await _bookingService.CreateBookingAsync(userId, createModel, cancellationToken);

            return CreatedAtAction(nameof(GetMyBookings), new { id = booking.Id }, booking);
        }

        [HttpGet("my")]
        public async Task<ActionResult<List<BookingDTO>>> GetMyBookings(CancellationToken cancellationToken)
        {
            var userId = await FindUserIdAsync();

            var bookings = await _bookingService.GetMyBookingsAsync(userId, cancellationToken);

            return Ok(bookings);
        }

        private async Task<Guid> FindUserIdAsync()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                throw new UnauthorizedException("Invalid user token.");
            }
            return userId;
        }
    }
}
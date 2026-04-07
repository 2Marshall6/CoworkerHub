using CoworkerHub.Application.DTOs.Booking;
using FluentValidation;

namespace CoworkerHub.Application.Validations
{
    public class CreateBookingValidator : AbstractValidator<CreateBookingDTO>
    {
        public CreateBookingValidator()
        {
            RuleFor(x => x.DeskId).GreaterThan(0);

            RuleFor(x => x.StartTime)
                .GreaterThan(DateTime.UtcNow.AddMinutes(-5))
                .WithMessage("Booking start time cannot be in the past.");

            RuleFor(x => x.EndTime)
                .Must((model, endTime) => endTime >= model.StartTime.AddHours(1))
                .WithMessage("Minimum booking time is 1 hour.");
        }
    }
}
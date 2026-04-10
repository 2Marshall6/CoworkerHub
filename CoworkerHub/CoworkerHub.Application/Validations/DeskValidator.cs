using CoworkerHub.Application.DTOs.Desk;
using FluentValidation;

namespace CoworkerHub.Application.Validations
{
    public class CreateDeskValidator : AbstractValidator<CreateDeskDTO>
    {
        public CreateDeskValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Floor).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Capacity).GreaterThan(0);
            RuleFor(x => x.PricePerHour).GreaterThan(0);
            RuleFor(x => x.WorkspaceId).GreaterThan(0);
        }
    }

    public class UpdateDeskValidator : AbstractValidator<UpdateDeskDTO>
    {
        public UpdateDeskValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Floor).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Capacity).GreaterThan(0);
            RuleFor(x => x.PricePerHour).GreaterThan(0);
        }
    }
}
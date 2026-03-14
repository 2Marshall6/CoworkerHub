using CoworkerHub.Application.DTOs;
using CoworkerHub.Domain.Entities;
using FluentValidation;

namespace CoworkerHub.Application.Validations
{
    public class RegisterUserValidator : AbstractValidator<RegisterUserDTO>
    {
        public RegisterUserValidator()
        {
            RuleFor(user => user.UserName).NotEmpty();
            RuleFor(user => user.Email).NotEmpty().EmailAddress();
            RuleFor(user => user.Password).MinimumLength(6).MaximumLength(50);
        }
    }
}

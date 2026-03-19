using CoworkerHub.Application.DTOs.Authentication;
using FluentValidation;


namespace CoworkerHub.Application.Validations
{
    public class LoginUserValidator : AbstractValidator<LoginUserDTO>
    {
        public LoginUserValidator()
        {
            RuleFor(user => user.Email).NotEmpty().EmailAddress();
            RuleFor(user => user.Password).NotEmpty().MinimumLength(6);
        }
    }
}

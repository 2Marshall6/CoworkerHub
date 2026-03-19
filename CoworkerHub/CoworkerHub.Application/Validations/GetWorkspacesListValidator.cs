using CoworkerHub.Application.DTOs.Authentication;
using CoworkerHub.Application.DTOs.Workspace;
using FluentValidation;

namespace CoworkerHub.Application.Validations
{
    public class GetWorkspacesListValidator : AbstractValidator<GetWorkspacesListDTO>
    {
        public GetWorkspacesListValidator()
        {
            RuleFor(dto => dto.PageNumber).GreaterThan(0);
            RuleFor(dto => dto.PageSize).GreaterThan(0).LessThan(1000);
        }
    }
}

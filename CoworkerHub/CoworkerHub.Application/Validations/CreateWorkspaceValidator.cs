using CoworkerHub.Application.DTOs;
using FluentValidation;

namespace CoworkerHub.Application.Validations
{
    public class CreateWorkspaceValidator : AbstractValidator<CreateWorkspaceDTO>
    {
        public CreateWorkspaceValidator()
        {
            RuleFor(workspace =>  workspace.Name).NotEmpty();
            RuleFor(workspace => workspace.Description).MinimumLength(3).NotEmpty();
        }
    }
}

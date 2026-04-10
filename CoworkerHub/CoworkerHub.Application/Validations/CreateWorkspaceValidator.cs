using CoworkerHub.Application.DTOs.Workspace;
using FluentValidation;

namespace CoworkerHub.Application.Validations
{
    public class CreateWorkspaceValidator : AbstractValidator<CreateWorkspaceDTO>
    {
        public CreateWorkspaceValidator()
        {
            RuleFor(workspace =>  workspace.Name).NotEmpty();
            RuleFor(workspace => workspace.Description).MinimumLength(3);
        }
    }

    public class GetWorkspacesListValidator : AbstractValidator<GetWorkspacesListDTO>
    {
        public GetWorkspacesListValidator()
        {
            RuleFor(dto => dto.PageNumber).GreaterThan(0);
            RuleFor(dto => dto.PageSize).GreaterThan(0).LessThan(1000);
        }
    }
}

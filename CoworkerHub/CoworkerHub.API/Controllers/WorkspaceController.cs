using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CoworkerHub.Application.Exceptions;
using CoworkerHub.Application.DTOs.Workspace;
using CoworkerHub.Domain.Entities;
using CoworkerHub.Application.Options;
using CoworkerHub.Application.Interfaces;

namespace CoworkerHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WorkspacesController : ControllerBase
    {
        private readonly IWorkspaceService _workspaceService;
        private readonly IValidator<CreateWorkspaceDTO> _validator;

        public WorkspacesController(IWorkspaceService workspaceService, IValidator<CreateWorkspaceDTO> validator)
        {
            _workspaceService = workspaceService;
            _validator = validator;
        }

        [HttpGet]
        public async Task<ActionResult<PageModel<WorkspaceDTO>>> GetAll([FromQuery] GetWorkspacesListDTO getWorkspacesListDTO, CancellationToken cancellationToken)
        {
            var workspaces = await _workspaceService.GetAllWorkspacesAsync(getWorkspacesListDTO, cancellationToken);
            return Ok(workspaces); 
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WorkspaceDTO>> GetById(int id, CancellationToken cancellationToken)
        {
            var workspace = await _workspaceService.GetWorkspaceByIdAsync(id, cancellationToken); 

            return Ok(workspace);
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateWorkspaceDTO createModel, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(createModel, cancellationToken);
           
            if (!validationResult.IsValid)
            {
                throw new AppValidationException(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            var workspace = await _workspaceService.CreateWorkspaceAsync(createModel, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = workspace.Id }, workspace);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _workspaceService.DeleteWorkspaceAsync(id, cancellationToken);
            
            return NoContent();
        }
    }
}
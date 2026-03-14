using CoworkerHub.Application.DTOs;
using CoworkerHub.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult<List<WorkspaceDTO>>> GetAll(CancellationToken cancellationToken)
        {
            var workspaces = await _workspaceService.GetAllWorkspacesAsync(cancellationToken);
            return Ok(workspaces); 
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WorkspaceDTO>> GetById(int id, CancellationToken cancellationToken)
        {
            var workspace = await _workspaceService.GetWorkspaceByIdAsync(id, cancellationToken); 

            if (workspace == null)
            {
                return NotFound($"Workspace with id {id} not found");
            }

            return Ok(workspace);
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateWorkspaceDTO createModel, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(createModel, cancellationToken);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var workspace = await _workspaceService.CreateWorkspaceAsync(createModel, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = workspace.Id }, workspace);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var isDeleted = await _workspaceService.DeleteWorkspaceAsync(id, cancellationToken);
            if (!isDeleted)
            {
                return NotFound($"Workspace with id {id} not found");
            }
            return NoContent();
        }
    }
}
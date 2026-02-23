using CoworkerHub.Application.DTOs;
using CoworkerHub.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CoworkerHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public async Task<ActionResult<List<WorkspaceDTO>>> GetAll()
        {
            var workspaces = await _workspaceService.GetAllWorkspacesAsync();
            return Ok(workspaces); 
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WorkspaceDTO>> GetById(int id)
        {
            var workspace = await _workspaceService.GetWorkspaceByIdAsync(id); 

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

            await _workspaceService.CreateWorkspaceAsync(createModel, cancellationToken);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var workspace = await _workspaceService.GetWorkspaceByIdAsync(id);
            if (workspace!=null)
            {
                await _workspaceService.DeleteWorkspaceAsync(id, cancellationToken);
                return Ok();
            }

            return NoContent();
        }
    }
}
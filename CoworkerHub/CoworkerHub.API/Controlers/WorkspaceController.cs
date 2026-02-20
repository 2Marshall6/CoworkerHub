using CoworkerHub.Core.DTO_s;
using CoworkerHub.Core.Entities;
using CoworkerHub.Core.Interfaces;
using CoworkerHub.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace CoworkerHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkspacesController : ControllerBase
    {
        private readonly IWorkspaceService _workspaceService;

        public WorkspacesController(IWorkspaceService workspaceService)
        {
            _workspaceService = workspaceService;
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
        public async Task<ActionResult<Workspace>> Create(CreateWorkspaceModel createModel)
        {
            await _workspaceService.CreateWorkspaceAsync(createModel);

            return CreatedAtAction(nameof(GetById), new { id = createModel.Id }, createModel);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _workspaceService.DeleteWorkspaceAsync(id);
            return NoContent(); 
        }
    }

    
}
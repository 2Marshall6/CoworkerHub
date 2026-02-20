using CoworkerHub.Core.DTO_s;
using CoworkerHub.Core.Entities;
using CoworkerHub.Core.Interfaces;
using CoworkerHub.Core.Models;

namespace CoworkerHub.Application
{
    public class WorkspaceService : IWorkspaceService
    {
        readonly IWorkspaceRepository _workspaceRepository;
        public WorkspaceService(IWorkspaceRepository workspaceRepository)
        {
            _workspaceRepository = workspaceRepository;
        }

        public async Task CreateWorkspaceAsync(CreateWorkspaceModel createModel)
        {
            var workspace = new Workspace
            {
                Name = createModel.Name,
                Description = createModel.Description
            };

            await _workspaceRepository.CreateWorkspaceAsync(workspace);
        }

        public async Task DeleteWorkspaceAsync(int deleteId)
        {
            await _workspaceRepository.DeleteWorkspaceAsync(deleteId);
        }

        public async Task<List<WorkspaceDTO>> GetAllWorkspacesAsync()
        {
            var workspaces = await _workspaceRepository.GetAllWorkspacesAsync();
            List<WorkspaceDTO> workspaceDTOList = new List<WorkspaceDTO>();

            foreach (var workspace in workspaces)
            {
                workspaceDTOList.Add(new WorkspaceDTO
                {
                    Id = workspace.Id,
                    Name = workspace.Name,
                    Description = workspace.Description
                });
            }
            return workspaceDTOList;
        }

        public async Task<WorkspaceDTO> GetWorkspaceByIdAsync(int getId)
        {
            var workspace = await _workspaceRepository.GetWorkspaceByIdAsync(getId);
            var workspaceDTO = new WorkspaceDTO
            {
                Id = workspace.Id,
                Name = workspace.Name,
                Description = workspace.Description
            };

            return workspaceDTO;
        }
    }
}

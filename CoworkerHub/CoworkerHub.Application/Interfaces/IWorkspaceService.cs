using CoworkerHub.Application.DTOs;

namespace CoworkerHub.Application.Interfaces
{
    public interface IWorkspaceService
    {
        public Task<List<WorkspaceDTO>> GetAllWorkspacesAsync();
        public Task<WorkspaceDTO> GetWorkspaceByIdAsync(int id);
        public Task CreateWorkspaceAsync(CreateWorkspaceDTO workspace, CancellationToken cancellationToken);
        public Task DeleteWorkspaceAsync(int Id, CancellationToken cancellationToken);
    }
}

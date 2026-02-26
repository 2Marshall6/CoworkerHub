using CoworkerHub.Application.DTOs;
using CoworkerHub.Domain.Entities;

namespace CoworkerHub.Application.Interfaces
{
    public interface IWorkspaceService
    {
        Task<List<WorkspaceDTO>> GetAllWorkspacesAsync();
        Task<WorkspaceDTO> GetWorkspaceByIdAsync(int id);
        Task<Workspace> CreateWorkspaceAsync(CreateWorkspaceDTO workspace, CancellationToken cancellationToken);
        Task DeleteWorkspaceAsync(int Id, CancellationToken cancellationToken);
    }
}

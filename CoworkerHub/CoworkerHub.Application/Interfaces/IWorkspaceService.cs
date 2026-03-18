using CoworkerHub.Application.DTOs;
using CoworkerHub.Domain.Entities;

namespace CoworkerHub.Application.Interfaces
{
    public interface IWorkspaceService
    {
        Task<List<WorkspaceDTO>> GetAllWorkspacesAsync(CancellationToken cancellationToken);
        Task<WorkspaceDTO?> GetWorkspaceByIdAsync(int id, CancellationToken cancellationToken);
        Task<WorkspaceDTO> CreateWorkspaceAsync(CreateWorkspaceDTO workspace, CancellationToken cancellationToken);
        Task DeleteWorkspaceAsync(int Id, CancellationToken cancellationToken);
    }
}

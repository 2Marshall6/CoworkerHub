using CoworkerHub.Application.DTOs.Workspace;
using CoworkerHub.Domain.Entities;


namespace CoworkerHub.Application.Interfaces
{
    public interface IWorkspaceRepository
    {
        Task<PageModel<Workspace>> GetAllWorkspacesAsync(GetWorkspacesListDTO getWorkspacesListDTO, CancellationToken cancellationToken);
        Task<Workspace?> GetWorkspaceByIdAsync(int id, CancellationToken cancellationToken);
        Task CreateWorkspaceAsync(Workspace workspace, CancellationToken cancellationToken);
        Task<int> DeleteWorkspaceAsync(int id, CancellationToken cancellationToken);
    }
}

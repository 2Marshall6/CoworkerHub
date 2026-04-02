using CoworkerHub.Application.Options;
using CoworkerHub.Domain.Entities;


namespace CoworkerHub.Application.Interfaces
{
    public interface IWorkspaceRepository
    {
        Task<PageModel<Workspace>> GetAllWorkspacesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<Workspace?> GetWorkspaceByIdAsync(int id, CancellationToken cancellationToken);
        Task CreateWorkspaceAsync(Workspace workspace, CancellationToken cancellationToken);
        Task<int> DeleteWorkspaceAsync(int id, CancellationToken cancellationToken);
    }
}

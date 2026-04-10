using CoworkerHub.Application.Options;
using CoworkerHub.Domain.Entities;


namespace CoworkerHub.Application.Interfaces
{
    public interface IWorkspaceRepository
    {
        Task<PageModel<Workspace>> GetAllWorkspacesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<Workspace?> GetWorkspaceByIdAsync(int id, CancellationToken cancellationToken);
        void CreateWorkspace(Workspace workspace);
        void DeleteWorkspace(Workspace workspace);
    }
}

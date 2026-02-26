using CoworkerHub.Domain.Entities;


namespace CoworkerHub.Application.Interfaces
{
    public interface IWorkspaceRepository
    {
        Task<List<Workspace>> GetAllWorkspacesAsync();
        Task<Workspace> GetWorkspaceByIdAsync(int id);
        Task CreateWorkspaceAsync(Workspace workspace, CancellationToken cancellationToken);
        Task DeleteWorkspaceAsync(int id, CancellationToken cancellationToken);
    }
}

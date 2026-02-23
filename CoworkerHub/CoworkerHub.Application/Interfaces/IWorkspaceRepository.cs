using CoworkerHub.Domain.Entities;


namespace CoworkerHub.Application.Interfaces
{
    public interface IWorkspaceRepository
    {
        public Task<List<Workspace>> GetAllWorkspacesAsync();
        public Task<Workspace> GetWorkspaceByIdAsync(int id);
        public Task CreateWorkspaceAsync(Workspace workspace, CancellationToken cancellationToken);
        public Task DeleteWorkspaceAsync(int id, CancellationToken cancellationToken);
    }
}

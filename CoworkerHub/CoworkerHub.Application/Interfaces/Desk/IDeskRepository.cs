using CoworkerHub.Domain.Entities;

namespace CoworkerHub.Application.Interfaces
{
    public interface IDeskRepository
    {
        void AddDesk(Desk desk); // Синхронный метод (как обсуждали в ревью)
        void DeleteDesk(Desk desk);
        Task<Desk?> GetDeskByIdAsync(int id, CancellationToken cancellationToken);
        Task<List<Desk>> GetDesksByWorkspaceIdAsync(int workspaceId, CancellationToken cancellationToken);
    }
}
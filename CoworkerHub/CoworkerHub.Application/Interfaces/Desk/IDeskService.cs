using CoworkerHub.Application.DTOs.Desk;
using CoworkerHub.Domain.Enums;

namespace CoworkerHub.Application.Interfaces
{
    public interface IDeskService
    {
        Task<DeskDTO> CreateDeskAsync(CreateDeskDTO createModel, CancellationToken cancellationToken);
        Task<DeskDTO> GetDeskByIdAsync(int id, CancellationToken cancellationToken);
        Task UpdateDeskAsync(int id, UpdateDeskDTO updateModel, CancellationToken cancellationToken);
        Task DeleteDeskAsync(int id, CancellationToken cancellationToken);
        Task ChangeDeskStatusAsync(int id, DeskStatus newStatus, CancellationToken cancellationToken);
    }
}
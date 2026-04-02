using CoworkerHub.Application.Interfaces;
using CoworkerHub.Domain.Entities;
using CoworkerHub.Infrastructure.Persistens;
using Microsoft.EntityFrameworkCore;

namespace CoworkerHub.Infrastructure.Repositories
{
    public class DeskRepository : IDeskRepository
    {
        private readonly ApplicationDbContext _context;

        public DeskRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AddDesk(Desk desk)
        {
            _context.Desks.Add(desk);
        }

        public void DeleteDesk(Desk desk)
        {
            _context.Desks.Remove(desk);
        }

        public async Task<Desk?> GetDeskByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Desks
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        }

        public async Task<List<Desk>> GetDesksByWorkspaceIdAsync(int workspaceId, CancellationToken cancellationToken)
        {
            return await _context.Desks
                .AsNoTracking()
                .Where(d => d.WorkspaceId == workspaceId)
                .ToListAsync(cancellationToken);
        }
    }
}
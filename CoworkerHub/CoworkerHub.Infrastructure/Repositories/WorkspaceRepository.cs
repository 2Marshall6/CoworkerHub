using CoworkerHub.Application.DTOs.Workspace;
using CoworkerHub.Application.Interfaces;
using CoworkerHub.Application.Options;
using CoworkerHub.Domain.Entities;
using CoworkerHub.Domain.Enums;
using CoworkerHub.Infrastructure.Persistens;
using Microsoft.EntityFrameworkCore;

namespace CoworkerHub.Infrastructure.Repositories
{
    public class WorkspaceRepository : IWorkspaceRepository
    {
        private readonly ApplicationDbContext _context;
        public WorkspaceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void CreateWorkspace(Workspace createModel)
        {
            _context.Workspaces
                .Add(createModel);
        }

        public void DeleteWorkspace(Workspace workspace)
        {
            _context.Workspaces.Remove(workspace);
        }

        public async Task<PageModel<Workspace>> GetAllWorkspacesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var query = _context.Workspaces
                                .AsNoTracking();
            
            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query.Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync(cancellationToken);

            int? nextPage = pageNumber * pageSize < totalCount
                            ? pageNumber + 1
                            : null;

            return new PageModel<Workspace>
            (
                pageNumber,
                nextPage,
                totalCount,
                items
            );
        }

        public async Task<Workspace?> GetWorkspaceByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Workspaces
                .AsNoTracking()
                .Include(w => w.Desks.Where(d => d.Status == DeskStatus.Available))
                    .ThenInclude(d => d.Booking.Where(b => b.Status != BookingStatus.Completed))
                .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
        }
    }
}

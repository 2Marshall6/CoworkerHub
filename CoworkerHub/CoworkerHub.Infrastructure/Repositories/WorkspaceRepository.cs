using CoworkerHub.Application.Interfaces;
using CoworkerHub.Domain.Entities;
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

        public async Task CreateWorkspaceAsync(Workspace createModel, CancellationToken cancellationToken)
        {
            var workspsce = await _context.Workspaces
                .AddAsync(createModel, cancellationToken);
        }

        public async Task DeleteWorkspaceAsync(int workspaceId, CancellationToken cancellationToken)
        {
            await _context.Workspaces
                .Where(w => w.Id == workspaceId)
                .ExecuteDeleteAsync(cancellationToken);
        }

        public async Task<List<Workspace>> GetAllWorkspacesAsync()
        {
            var workspaces = await _context.Workspaces
                                    .AsNoTracking()
                                    .ToListAsync();
            return workspaces;
        }

        public async Task<Workspace> GetWorkspaceByIdAsync(int id)
        {
            var warkspace = await _context.Workspaces
                                    .AsNoTracking()
                                    .Where(w => w.Id == id)
                                    .FirstOrDefaultAsync();
            return warkspace;
        }
    }
}

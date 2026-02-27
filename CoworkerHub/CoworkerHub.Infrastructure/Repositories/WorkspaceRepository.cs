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
            var workspace = await _context.Workspaces
                .AddAsync(createModel, cancellationToken);
        }

        public async Task<int> DeleteWorkspaceAsync(int workspaceId, CancellationToken cancellationToken)
        {
            return await _context.Workspaces
                .Where(w => w.Id == workspaceId)
                .ExecuteDeleteAsync(cancellationToken);
        }

        public async Task<List<Workspace>> GetAllWorkspacesAsync(CancellationToken cancellationToken)
        {
            var workspaces = await _context.Workspaces
                                    .AsNoTracking()
                                    .ToListAsync(cancellationToken);
            return workspaces;
        }

        public async Task<Workspace?> GetWorkspaceByIdAsync(int id, CancellationToken cancellationToken)
        {
            var workspace = await _context.Workspaces
                                    .AsNoTracking()
                                    .Where(w => w.Id == id)
                                    .FirstOrDefaultAsync(cancellationToken);
            return workspace;
        }
    }
}

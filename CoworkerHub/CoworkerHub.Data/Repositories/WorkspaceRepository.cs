using CoworkerHub.Core.Entities;
using CoworkerHub.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoworkerHub.Data.Repositories
{
    public class WorkspaceRepository : IWorkspaceRepository
    {
        private readonly ApplicationDbContext _context;
        public WorkspaceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateWorkspaceAsync(Workspace createModel)
        {
            await _context.Workspaces
                .AddAsync(createModel);
            await _context
                .SaveChangesAsync();
        }

        public async Task DeleteWorkspaceAsync(int workspaceId)
        {
            await _context.Workspaces
                .Where(w => w.Id == workspaceId)
                .ExecuteDeleteAsync();
            await _context
                .SaveChangesAsync();
        }

        public async Task<List<Workspace>> GetAllWorkspacesAsync()
        {
            var workspaces = await _context.Workspaces
                                    .ToListAsync();
            return workspaces;
        }

        public async Task<Workspace> GetWorkspaceByIdAsync(int id)
        {
            var warkspace = await _context.Workspaces
                                    .Where(w => w.Id == id)
                                    .FirstOrDefaultAsync();
            return warkspace;
        }
    }
}

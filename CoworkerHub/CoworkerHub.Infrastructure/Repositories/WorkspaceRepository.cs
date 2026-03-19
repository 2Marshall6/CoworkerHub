using CoworkerHub.Application.DTOs.Workspace;
using CoworkerHub.Application.Interfaces;
using CoworkerHub.Domain.Entities;
using CoworkerHub.Infrastructure.Persistens;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
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
            await _context.Workspaces
                .AddAsync(createModel, cancellationToken);
        }

        public async Task<int> DeleteWorkspaceAsync(int workspaceId, CancellationToken cancellationToken)
        {
            return await _context.Workspaces
                .Where(w => w.Id == workspaceId)
                .ExecuteDeleteAsync(cancellationToken);
        }

        public async Task<PageModel<Workspace>> GetAllWorkspacesAsync(GetWorkspacesListDTO getWorkspacesListDTO, CancellationToken cancellationToken)
        {
            var query = _context.Workspaces
                                .AsNoTracking()
                                .AsQueryable();
            
            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query.Skip((getWorkspacesListDTO.PageNumber - 1) * getWorkspacesListDTO.PageSize)
                                .Take(getWorkspacesListDTO.PageSize)
                                .ToListAsync(cancellationToken);

            var nextPage = getWorkspacesListDTO.PageNumber * getWorkspacesListDTO.PageSize < totalCount
                            ? getWorkspacesListDTO.PageNumber + 1
                            : 0;

            return new PageModel<Workspace>
            (
                getWorkspacesListDTO.PageNumber,
                nextPage,
                totalCount,
                items
            );
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

using CoworkerHub.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoworkerHub.Core.Interfaces
{
    public interface IWorkspaceRepository
    {
        public Task<List<Workspace>> GetAllWorkspacesAsync();
        public Task<Workspace> GetWorkspaceByIdAsync(int id);
        public Task CreateWorkspaceAsync(Workspace workspace);
        public Task DeleteWorkspaceAsync(int workspaceId);
    }
}

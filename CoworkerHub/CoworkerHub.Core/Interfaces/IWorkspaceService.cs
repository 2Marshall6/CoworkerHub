using CoworkerHub.Core.DTO_s;
using CoworkerHub.Core.Entities;
using CoworkerHub.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoworkerHub.Core.Interfaces
{
    public interface IWorkspaceService
    {
        public Task<List<WorkspaceDTO>> GetAllWorkspacesAsync();
        public Task<WorkspaceDTO> GetWorkspaceByIdAsync(int id);
        public Task CreateWorkspaceAsync(CreateWorkspaceModel workspace);
        public Task DeleteWorkspaceAsync(int workspaceId);
    }
}

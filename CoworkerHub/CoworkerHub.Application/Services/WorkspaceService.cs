using AutoMapper;
using CoworkerHub.Application.DTOs;
using CoworkerHub.Application.Interfaces;
using CoworkerHub.Domain.Entities;
using System.Data;

namespace CoworkerHub.Application.Services
{
    public class WorkspaceService : IWorkspaceService
    {
        private readonly IWorkspaceRepository _workspaceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public WorkspaceService(IWorkspaceRepository workspaceRepository, IUnitOfWork unitOfWork,IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _workspaceRepository = workspaceRepository;
        }

        public async Task<WorkspaceDTO> CreateWorkspaceAsync(CreateWorkspaceDTO createModel, CancellationToken cancellationToken)
        {
            Workspace workspace = _mapper.Map<Workspace>(createModel);

            await _workspaceRepository.CreateWorkspaceAsync(workspace, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<WorkspaceDTO>(workspace);
        }

        public async Task<bool> DeleteWorkspaceAsync(int deleteId, CancellationToken cancellationToken)
        {
            var deletedrows = await _workspaceRepository.DeleteWorkspaceAsync(deleteId, cancellationToken);
            return deletedrows > 0;
        }

        public async Task<List<WorkspaceDTO>> GetAllWorkspacesAsync(CancellationToken cancellationToken)
        {
            var workspaces = await _workspaceRepository.GetAllWorkspacesAsync(cancellationToken);

            return _mapper.Map<List<WorkspaceDTO>>(workspaces);
        }

        public async Task<WorkspaceDTO?> GetWorkspaceByIdAsync(int getId, CancellationToken cancellationToken)
        {
            var workspace = await _workspaceRepository.GetWorkspaceByIdAsync(getId, cancellationToken);

            return workspace == null ? null: _mapper.Map<WorkspaceDTO>(workspace);
        }
    }
}

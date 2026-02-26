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

        public async Task<Workspace> CreateWorkspaceAsync(CreateWorkspaceDTO createModel, CancellationToken cancellationToken)
        {
            Workspace workspace = _mapper.Map<Workspace>(createModel);

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                await _workspaceRepository.CreateWorkspaceAsync(workspace, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync(CancellationToken.None);
                throw;
            }

            return workspace;
        }

        public async Task DeleteWorkspaceAsync(int deleteId, CancellationToken cancellationToken)
        {
            await _workspaceRepository.DeleteWorkspaceAsync(deleteId, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<WorkspaceDTO>> GetAllWorkspacesAsync()
        {
            var workspaces = await _workspaceRepository.GetAllWorkspacesAsync();
            List<WorkspaceDTO> workspaceDTOList = new List<WorkspaceDTO>();

            foreach (var workspace in workspaces)
            {
                var workspaceDTO = _mapper.Map<WorkspaceDTO>(workspace);
                workspaceDTOList.Add(workspaceDTO);
            }
            return workspaceDTOList;
        }

        public async Task<WorkspaceDTO> GetWorkspaceByIdAsync(int getId)
        {
            var workspace = await _workspaceRepository.GetWorkspaceByIdAsync(getId);
            var workspaceDTO = _mapper.Map<WorkspaceDTO>(workspace);

            return workspaceDTO;
        }
    }
}

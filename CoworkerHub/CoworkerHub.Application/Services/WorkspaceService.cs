using AutoMapper;
using CoworkerHub.Application.DTOs.Workspace;
using CoworkerHub.Application.Exceptions;
using CoworkerHub.Application.Interfaces;
using CoworkerHub.Application.Options;
using CoworkerHub.Domain.Entities;

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

            _workspaceRepository.CreateWorkspace(workspace);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<WorkspaceDTO>(workspace);
        }

        public async Task DeleteWorkspaceAsync(int deleteId, CancellationToken cancellationToken)
        {
            var workspace = await _workspaceRepository.GetWorkspaceByIdAsync(deleteId, cancellationToken);

            if (workspace == null)
            {
                throw new NotFoundException($"Workspace with id {deleteId} not found.");
            }

            _workspaceRepository.DeleteWorkspace(workspace);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<PageModel<WorkspaceDTO>> GetAllWorkspacesAsync(GetWorkspacesListDTO getWorkspacesListDTO, CancellationToken cancellationToken)
        {
            var workspacesPage = await _workspaceRepository.GetAllWorkspacesAsync(getWorkspacesListDTO.PageNumber,getWorkspacesListDTO.PageSize, cancellationToken);

            return _mapper.Map<PageModel<WorkspaceDTO>>(workspacesPage);
        }

        public async Task<WorkspaceDetailsDTO?> GetWorkspaceByIdAsync(int getId, CancellationToken cancellationToken)
        {
            var workspace = await _workspaceRepository.GetWorkspaceByIdAsync(getId, cancellationToken);
            if (workspace == null)
                throw new NotFoundException($"Workspace with id {getId} not found.");
            

            return _mapper.Map<WorkspaceDetailsDTO>(workspace);
        }
    }
}

using AutoMapper;
using CoworkerHub.Application.DTOs.Desk;
using CoworkerHub.Application.Exceptions;
using CoworkerHub.Application.Interfaces;
using CoworkerHub.Domain.Entities;
using CoworkerHub.Domain.Enums;

namespace CoworkerHub.Application.Services
{
    public class DeskService : IDeskService
    {
        private readonly IDeskRepository _deskRepository;
        private readonly IWorkspaceRepository _workspaceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DeskService(IDeskRepository deskRepository, IWorkspaceRepository workspaceRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _deskRepository = deskRepository;
            _workspaceRepository = workspaceRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<DeskDTO> CreateDeskAsync(CreateDeskDTO createModel, CancellationToken cancellationToken)
        {
            var workspace = await _workspaceRepository.GetWorkspaceByIdAsync(createModel.WorkspaceId, cancellationToken);
            if (workspace == null)
                throw new NotFoundException($"Workspace with ID {createModel.WorkspaceId} not found.");

            var desk =  _mapper.Map<Desk>(createModel);
            desk.Status = DeskStatus.Available;

            _deskRepository.AddDesk(desk);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<DeskDTO>(desk);
        }

        public async Task<DeskDTO> GetDeskByIdAsync(int id, CancellationToken cancellationToken)
        {
            var desk = await _deskRepository.GetDeskByIdAsync(id, cancellationToken);
            if (desk == null)
                throw new NotFoundException($"Desk with ID {id} not found.");

            return _mapper.Map<DeskDTO>(desk);
        }

        public async Task<List<DeskDTO>> GetDesksByWorkspaceIdAsync(int workspaceId, CancellationToken cancellationToken)
        {
            var desks = await _deskRepository.GetDesksByWorkspaceIdAsync(workspaceId, cancellationToken);
            return _mapper.Map<List<DeskDTO>>(desks);
        }

        public async Task UpdateDeskAsync(int id, UpdateDeskDTO updateModel, CancellationToken cancellationToken)
        {
            var desk = await _deskRepository.GetDeskByIdAsync(id, cancellationToken);
            if (desk == null)
                throw new NotFoundException($"Desk with ID {id} not found.");

            _mapper.Map(updateModel, desk);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteDeskAsync(int id, CancellationToken cancellationToken)
        {
            var desk = await _deskRepository.GetDeskByIdAsync(id, cancellationToken);
            if (desk == null)
                throw new NotFoundException($"Desk with ID {id} not found.");

            _deskRepository.DeleteDesk(desk);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task ChangeDeskStatusAsync(int id, DeskStatus newStatus, CancellationToken cancellationToken)
        {
            var desk = await _deskRepository.GetDeskByIdAsync(id, cancellationToken);
            if (desk == null)
                throw new NotFoundException($"Desk with ID {id} not found.");

            desk.Status = newStatus;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
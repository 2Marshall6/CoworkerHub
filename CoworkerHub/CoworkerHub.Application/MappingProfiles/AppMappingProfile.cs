using AutoMapper;
using CoworkerHub.Application.DTOs.Authentication;
using CoworkerHub.Application.DTOs.Workspace;
using CoworkerHub.Domain.Entities;

namespace CoworkerHub.Application.MappingProfiles
{
    public class AppMappingProfile : Profile
    {
        public AppMappingProfile()
        {
            CreateMap<Workspace, WorkspaceDTO>();
            CreateMap<CreateWorkspaceDTO, Workspace>();
            CreateMap<User, UserDTO>();
            CreateMap<RegisterUserDTO, User>();
            CreateMap<PageModel<Workspace>, PageModel<WorkspaceDTO>>();
        }
            
    }
}

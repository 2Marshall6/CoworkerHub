using AutoMapper;
using CoworkerHub.Application.DTOs;
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
        }
            
    }
}

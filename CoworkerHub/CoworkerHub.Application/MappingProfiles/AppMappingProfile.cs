using AutoMapper;
using CoworkerHub.Application.DTOs.Authentication;
using CoworkerHub.Application.DTOs.Booking;
using CoworkerHub.Application.DTOs.Desk;
using CoworkerHub.Application.DTOs.Workspace;
using CoworkerHub.Application.Options;
using CoworkerHub.Domain.Entities;

namespace CoworkerHub.Application.MappingProfiles
{
    public class AppMappingProfile : Profile
    {
        public AppMappingProfile()
        {
            CreateMap<Workspace, WorkspaceDetailsDTO>()
                .ForMember(
                    dest => dest.Desks,
                    opt => opt.MapFrom(src => src.Desks)); ;
            CreateMap<Workspace, WorkspaceDTO>();
            CreateMap<CreateWorkspaceDTO, Workspace>();
            CreateMap<User, UserDTO>();
            CreateMap<RegisterUserDTO, User>();
            CreateMap<PageModel<Workspace>, PageModel<WorkspaceDTO>>();

            CreateMap<Desk, DeskDTO>();
            CreateMap<CreateDeskDTO, Desk>();
            CreateMap<UpdateDeskDTO, Desk>();

            CreateMap<Booking, TimeSlotDTO>()
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime));

            CreateMap<Desk, DeskDTO>()
                .ForMember(
                    dest => dest.OccupiedTimeSlots,
                    opt => opt.MapFrom(src => src.Booking));

            CreateMap<Booking, BookingDTO>();
        }
    }
}

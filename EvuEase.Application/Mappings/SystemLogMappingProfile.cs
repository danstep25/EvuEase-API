using AutoMapper;
using EvuEase.Application.DTOs.SystemLog;
using EvuEase.Domain.Entities;

namespace EvuEase.Application.Mappings;

public class SystemLogMappingProfile : Profile
{
    public SystemLogMappingProfile()
    {
        CreateMap<SystemLog, SystemLogResponse>()
            .ForMember(dest => dest.LogId, opt => opt.MapFrom(src => src.log_id))
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.user))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.role))
            .ForMember(dest => dest.Action, opt => opt.MapFrom(src => src.action))
            .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.timestamp))
            .ForMember(dest => dest.Module, opt => opt.MapFrom(src => src.module))
            .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.details))
            .ForMember(dest => dest.IpAddress, opt => opt.MapFrom(src => src.ip_address));
    }
}


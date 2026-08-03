using AutoMapper;
using EvuEase.Application.DTOs.SyTerm;
using EvuEase.Domain.Entities;

namespace EvuEase.Application.Mappings;

public class SyTermMappingProfile : Profile
{
    public SyTermMappingProfile()
    {
        CreateMap<SyTerm, SyTermResponse>()
            .ForMember(dest => dest.SyId, opt => opt.MapFrom(src => src.sy_id))
            .ForMember(dest => dest.SyCode, opt => opt.MapFrom(src => src.sy_code))
            .ForMember(dest => dest.SyYear, opt => opt.MapFrom(src => src.sy_year))
            .ForMember(dest => dest.SySemester, opt => opt.MapFrom(src => src.sy_semester))
            .ForMember(dest => dest.SyStartDate, opt => opt.MapFrom(src => src.sy_startdate))
            .ForMember(dest => dest.SyEndDate, opt => opt.MapFrom(src => src.sy_enddate))
            .ForMember(dest => dest.SyEnrollmentStart, opt => opt.MapFrom(src => src.sy_enrollmentstart))
            .ForMember(dest => dest.SyEnrollmentEnd, opt => opt.MapFrom(src => src.sy_enrollmentend))
            .ForMember(dest => dest.SyStatus, opt => opt.MapFrom(src => src.sy_status))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.created_at))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.updated_at));
    }
}


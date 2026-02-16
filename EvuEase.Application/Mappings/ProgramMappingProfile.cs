using AutoMapper;
using EvuEase.Application.DTOs.Program;
using EvuEase.Domain.Entities;

namespace EvuEase.Application.Mappings;

public class ProgramMappingProfile : Profile
{
    public ProgramMappingProfile()
    {
        CreateMap<Program, ProgramResponse>()
            .ForMember(dest => dest.ProgramId, opt => opt.MapFrom(src => src.program_id))
            .ForMember(dest => dest.ProgramCode, opt => opt.MapFrom(src => src.program_code))
            .ForMember(dest => dest.ProgramTitle, opt => opt.MapFrom(src => src.program_title))
            .ForMember(dest => dest.ProgramCompletionYears, opt => opt.MapFrom(src => src.program_completionyears))
            .ForMember(dest => dest.ProgramTotalUnits, opt => opt.MapFrom(src => src.program_totalunits))
            .ForMember(dest => dest.ProgramStatus, opt => opt.MapFrom(src => src.program_status))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.created_at))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.updated_at));
    }
}



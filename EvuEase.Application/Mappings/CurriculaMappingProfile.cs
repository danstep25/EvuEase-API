using AutoMapper;
using EvuEase.Application.DTOs.Curricula;
using EvuEase.Domain.Entities;

namespace EvuEase.Application.Mappings;

public class CurriculaMappingProfile : Profile
{
    public CurriculaMappingProfile()
    {
        CreateMap<Curricula, CurriculaResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
            .ForMember(dest => dest.CurriculumCode, opt => opt.MapFrom(src => src.curriculum_code))
            .ForMember(dest => dest.Version, opt => opt.MapFrom(src => src.version))
            .ForMember(dest => dest.ProgramId, opt => opt.MapFrom(src => src.program_id))
            .ForMember(dest => dest.SyId, opt => opt.MapFrom(src => src.sy_id))
            .ForMember(dest => dest.EffectiveDate, opt => opt.MapFrom(src => src.effective_date))
            .ForMember(dest => dest.CurriculumStatus, opt => opt.MapFrom(src => src.curriculum_status))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.created_at))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.updated_at));
    }
}




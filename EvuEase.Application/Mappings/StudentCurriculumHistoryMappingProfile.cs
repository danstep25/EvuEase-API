using AutoMapper;
using EvuEase.Application.DTOs.Student;
using EvuEase.Domain.Entities;

namespace EvuEase.Application.Mappings;

public class StudentCurriculumHistoryMappingProfile : Profile
{
    public StudentCurriculumHistoryMappingProfile()
    {
        CreateMap<StudentCurriculumHistory, StudentCurriculumHistoryResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
            .ForMember(dest => dest.CurriculumCode, opt => opt.MapFrom(src => src.curriculum_code))
            .ForMember(dest => dest.EffectiveSchoolYear, opt => opt.MapFrom(src => src.effective_school_year))
            .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.reason))
            .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.notes))
            .ForMember(dest => dest.MigratedBy, opt => opt.MapFrom(src => src.migrated_by))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.created_at))
            .ForMember(dest => dest.IsCurrent, opt => opt.Ignore());
    }
}

using AutoMapper;
using EvuEase.Application.DTOs.Student;
using EvuEase.Domain.Entities;

namespace EvuEase.Application.Mappings;

public class StudentMappingProfile : Profile
{
    public StudentMappingProfile()
    {
        CreateMap<Student, StudentResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
            .ForMember(dest => dest.StudentNumber, opt => opt.MapFrom(src => src.student_number))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.first_name))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.last_name))
            .ForMember(dest => dest.MiddleName, opt => opt.MapFrom(src => src.middle_name))
            .ForMember(dest => dest.ProgramCode, opt => opt.MapFrom(src => src.program_code))
            .ForMember(dest => dest.ProgramTitle, opt => opt.MapFrom(src => src.program_title))
            .ForMember(dest => dest.YearLevel, opt => opt.MapFrom(src => src.year_level))
            .ForMember("Type", opt => opt.MapFrom(src => src.student_type))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.enrollment_status))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.address))
            .ForMember(dest => dest.ContactNumber, opt => opt.MapFrom(src => src.contact_number))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.email))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.gender))
            .ForMember(dest => dest.Birthdate, opt => opt.MapFrom(src => src.birthdate))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.created_at))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.updated_at));
    }
}

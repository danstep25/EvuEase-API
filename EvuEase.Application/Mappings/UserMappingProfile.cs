using AutoMapper;
using EvuEase.Application.DTOs.User;
using EvuEase.Domain.Entities;

namespace EvuEase.Application.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        // User Entity -> UserResponse DTO
        CreateMap<User, UserResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.email))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.role))
            .ForMember(dest => dest.EmailVerifiedAt, opt => opt.MapFrom(src => src.email_verified_at))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.created_at));

        // Reverse mapping (if needed for updates)
        CreateMap<UserResponse, User>()
            .ForMember(dest => dest.id, opt => opt.Ignore()) // ID should not be mapped from DTO
            .ForMember(dest => dest.password, opt => opt.Ignore()) // Password should not be mapped
            .ForMember(dest => dest.remember_token, opt => opt.Ignore())
            .ForMember(dest => dest.updated_at, opt => opt.Ignore())
            .ForMember(dest => dest.created_at, opt => opt.Ignore());
    }
}


using AutoMapper;
using EvuEase.Application.DTOs.User;
using EvuEase.Domain.Entities;

namespace EvuEase.Application.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.email))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.role))
            .ForMember(dest => dest.EmailVerifiedAt, opt => opt.MapFrom(src => src.email_verified_at))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.created_at))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.status ? 1 : 0));

        CreateMap<UserResponse, User>()
            .ForMember(dest => dest.id, opt => opt.Ignore())
            .ForMember(dest => dest.password, opt => opt.Ignore())
            .ForMember(dest => dest.remember_token, opt => opt.Ignore())
            .ForMember(dest => dest.updated_at, opt => opt.Ignore())
            .ForMember(dest => dest.created_at, opt => opt.Ignore());

        CreateMap<CreateUserRequest, User>()
            .ForMember(dest => dest.id, opt => opt.Ignore())
            .ForMember(dest => dest.password, opt => opt.Ignore())
            .ForMember(dest => dest.remember_token, opt => opt.Ignore())
            .ForMember(dest => dest.updated_at, opt => opt.Ignore())
            .ForMember(dest => dest.status, opt => opt.Ignore())
            .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.password, opt => opt.MapFrom( src => src.Password))
            .ForMember(dest => dest.email, opt => opt.MapFrom( src => src.Email)) 
            .ForMember(dest => dest.created_at, opt => opt.Ignore());
    }
}





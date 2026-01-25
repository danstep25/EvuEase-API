using AutoMapper;
using EvuEase.Application.Common;
using EvuEase.Application.DTOs.User;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Application.Interfaces.Services;
using EvuEase.Domain.Entities;

namespace EvuEase.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<PagedResults<UserResponse>> AllUsers(UserRequest userRequest)
    {
        var pagedEntities = await _userRepository.GetAllUsers(userRequest);
        return pagedEntities.MapToDto<User, UserResponse>(_mapper);
    }
}
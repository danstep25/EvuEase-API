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

    public async Task<string> CreateUserAsync(CreateUserRequest userRequest)
    {
        var user = User.Create(
            userRequest.FullName,
            userRequest.Email,
            userRequest.Password.Hash(),
            userRequest.Role,
            userRequest.Status
            );
        var result = await _userRepository.CreateUserAsync(user);
        return result.email;
    }

    public async Task<string> UpdateUserAsync(UpdateUserRequest userRequest)
    {
        var user = await _userRepository.GetUserByIdAsync(userRequest.Id);

        if (user == null)
        {
            throw new Exception("User not found");
        }

        user.Update(
            userRequest.FullName,
            userRequest.Email,
            userRequest.Role,
            userRequest.Status
            );
        
        var result = await _userRepository.UpdateUserAsync(user);
        return result.email;
    }

    public async Task<UserStatisticsResponse> GetStatisticsAsync()
    {
        return await _userRepository.GetStatisticsAsync();
    }
}
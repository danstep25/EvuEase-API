using EvuEase.Application.Common;
using EvuEase.Application.DTOs.User;

namespace EvuEase.Application.Interfaces.Services;

public interface IUserService
{
    Task<PagedResults<UserResponse>> AllUsers(UserRequest userRequest);
    Task<String> CreateUserAsync(CreateUserRequest userRequest);
    Task<string> UpdateUserAsync(UpdateUserRequest userRequest);
    Task<UserStatisticsResponse> GetStatisticsAsync();
}


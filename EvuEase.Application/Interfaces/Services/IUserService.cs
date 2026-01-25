using EvuEase.Application.Common;
using EvuEase.Application.DTOs.User;

namespace EvuEase.Application.Interfaces.Services;

public interface IUserService
{
    Task<PagedResults<UserResponse>> AllUsers(UserRequest userRequest);
}


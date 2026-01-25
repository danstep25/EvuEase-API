using EvuEase.Application.DTOs.User;
using EvuEase.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace EvuEase.API.Controller;

[Route("api/[controller]")]
public class UserController : BaseController
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] UserRequest userRequest)
    {
        try
        {
            var data = await _userService.AllUsers(userRequest);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while retrieving users.", ex.Message);
        }
    }
}


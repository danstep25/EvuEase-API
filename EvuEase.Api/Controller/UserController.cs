using Microsoft.AspNetCore.Mvc;
using EvuEase.Application.Interfaces.Services;

namespace EvuEase.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<List<string>>> Get()
        {
            var data = await _userService.AllUsers();
            return Ok(data);
        }
    }
}


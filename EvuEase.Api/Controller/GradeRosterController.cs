using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EvuEase.API.Controller;




[Route("api/[controller]")]
public class GradeRosterController : BaseController
{
    
    
    
    [Authorize]
    [HttpGet]
    public IActionResult GetGradeRosters()
    {
        return Ok(Array.Empty<object>());
    }
}

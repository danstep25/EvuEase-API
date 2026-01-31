using EvuEase.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace EvuEase.API.Controller;

[Route("api/[controller]")]
public class LookupController : BaseController
{
    private readonly ILookupService _lookupService;

    public LookupController(ILookupService lookupService)
    {
        _lookupService = lookupService;
    }

    [HttpGet("modules")]
    public async Task<IActionResult> GetModules()
    {
        try
        {
            var data = await _lookupService.GetModuleLookupAsync();
            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while retrieving module lookup.", ex.Message);
        }
    }
}


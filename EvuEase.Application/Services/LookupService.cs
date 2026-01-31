using EvuEase.Application.DTOs;
using EvuEase.Application.Interfaces.Services;
using EvuEase.Domain.Enums;

namespace EvuEase.Application.Services;

public class LookupService : ILookupService
{
    public Task<List<LookupResponse>> GetModuleLookupAsync()
    {
        var modules = Enum.GetValues(typeof(Module))
            .Cast<Module>()
            .Select(module => new LookupResponse
            {
                Id = (int)module,
                Value = module.GetDescription()
            })
            .ToList();

        return Task.FromResult(modules);
    }
}


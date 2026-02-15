using EvuEase.Application.DTOs;

namespace EvuEase.Application.Interfaces.Services;

public interface ILookupService
{
    Task<List<LookupResponse>> GetModuleLookupAsync();
}


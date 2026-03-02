using EvuEase.Application.DTOs;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Application.Interfaces.Services;
using EvuEase.Domain.Enums;

namespace EvuEase.Application.Services;

public class LookupService : ILookupService
{
    private readonly IProgramRepository _programRepository;
    private readonly ISyTermRepository _syTermRepository;
    private readonly ICurriculaRepository _curriculaRepository;
    private readonly ICourseRepository _courseRepository;

    public LookupService(
        IProgramRepository programRepository,
        ISyTermRepository syTermRepository,
        ICurriculaRepository curriculaRepository,
        ICourseRepository courseRepository)
    {
        _programRepository = programRepository;
        _syTermRepository = syTermRepository;
        _curriculaRepository = curriculaRepository;
        _courseRepository = courseRepository;
    }

    public async Task<List<LookupResponse>> GetModuleLookupAsync()
    {
        try
        {
            var moduleValues = Enum.GetValues(typeof(Module))
                .Cast<Module>()
                .Where(module => Enum.IsDefined(typeof(Module), module))
                .ToList();

            if (!moduleValues.Any())
            {
                return new List<LookupResponse>();
            }

            var lookupResults = moduleValues
                .Select(module => new LookupResponse
                {
                    Id = (int)module,
                    Value = module.GetDescription(),
                    DisplayText = module.GetDescription()
                })
                .OrderBy(m => m.Value)
                .ToList();

            return await Task.FromResult(lookupResults);
        }
        catch (Exception)
        {
            return new List<LookupResponse>();
        }
    }

    public async Task<List<LookupResponse>> GetProgramsLookupAsync()
    {
        try
        {
            var lookupItems = await _programRepository.GetLookupItemsAsync();

            return lookupItems.Select(item => new LookupResponse
            {
                Id = (int)item.Id,
                Value = item.Value,
                DisplayText = item.DisplayText
            }).ToList();
        }
        catch (Exception)
        {
            return new List<LookupResponse>();
        }
    }

    public async Task<List<LookupResponse>> GetSyTermsLookupAsync()
    {
        try
        {
            var lookupItems = await _syTermRepository.GetLookupItemsAsync();

            return lookupItems.Select(item => new LookupResponse
            {
                Id = (int)item.Id,
                Value = item.Value,
                DisplayText = item.DisplayText
            }).ToList();
        }
        catch (Exception)
        {
            return new List<LookupResponse>();
        }
    }

    public async Task<List<LookupResponse>> GetCurriculaLookupAsync(long? programId = null)
    {
        try
        {
            var lookupItems = await _curriculaRepository.GetLookupItemsAsync(programId);

            return lookupItems.Select(item => new LookupResponse
            {
                Id = (int)item.Id,
                Value = item.Value,
                DisplayText = item.DisplayText
            }).ToList();
        }
        catch (Exception)
        {
            return new List<LookupResponse>();
        }
    }

    public async Task<List<LookupResponse>> GetCurriculumVersionsLookupAsync(string programCode)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(programCode))
            {
                return new List<LookupResponse>();
            }

            var lookupItems = await _curriculaRepository.GetCurriculumVersionsByProgramCodeAsync(programCode);

            return lookupItems.Select(item => new LookupResponse
            {
                Id = (int)item.Id,
                Value = item.Value,
                DisplayText = item.DisplayText
            }).ToList();
        }
        catch (Exception)
        {
            return new List<LookupResponse>();
        }
    }

    public async Task<List<LookupResponse>> GetCoursesLookupAsync()
    {
        try
        {
            var lookupItems = await _courseRepository.GetLookupItemsAsync();

            return lookupItems.Select(item => new LookupResponse
            {
                Id = (int)item.Id,
                Value = item.Value,
                DisplayText = item.DisplayText
            }).ToList();
        }
        catch (Exception)
        {
            return new List<LookupResponse>();
        }
    }
}


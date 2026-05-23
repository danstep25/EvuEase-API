using AutoMapper;
using EvuEase.Application.Common;
using EvuEase.Application.DTOs.Curricula;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Application.Interfaces.Services;
using EvuEase.Domain.Entities;

namespace EvuEase.Application.Services;

public class CurriculaService : ICurriculaService
{
    private readonly ICurriculaRepository _curriculaRepository;
    private readonly IProgramRepository _programRepository;
    private readonly ISyTermRepository _syTermRepository;
    private readonly IMapper _mapper;

    public CurriculaService(
        ICurriculaRepository curriculaRepository,
        IProgramRepository programRepository,
        ISyTermRepository syTermRepository,
        IMapper mapper)
    {
        _curriculaRepository = curriculaRepository;
        _programRepository = programRepository;
        _syTermRepository = syTermRepository;
        _mapper = mapper;
    }

    public async Task<PagedResults<CurriculaResponse>> GetAllCurricula(CurriculaRequest curriculaRequest)
    {
        var pagedEntities = await _curriculaRepository.GetAllCurricula(curriculaRequest);
        var curriculaList = pagedEntities.Result.ToList();
        
        var responseList = new List<CurriculaResponse>();
        foreach (var curricula in curriculaList)
        {
            var response = _mapper.Map<CurriculaResponse>(curricula);
            
            var program = await _programRepository.GetProgramByIdAsync(curricula.program_id);
            if (program != null)
            {
                response.ProgramCode = program.program_code;
                response.ProgramTitle = program.program_title;
            }
            
            var syTerm = await _syTermRepository.GetSyTermByIdAsync(curricula.sy_id);
            if (syTerm != null)
            {
                response.SyYear = syTerm.sy_year;
            }
            
            responseList.Add(response);
        }
        
        return new PagedResults<CurriculaResponse>(
            pagedEntities.PageIndex,
            pagedEntities.PageSize,
            pagedEntities.TotalRecords,
            pagedEntities.TotalEntries,
            responseList
        );
    }

    public async Task<CurriculaResponse?> GetCurriculaByIdAsync(long id)
    {
        var curricula = await _curriculaRepository.GetCurriculaByIdAsync(id);
        if (curricula == null)
        {
            return null;
        }
        
        var response = _mapper.Map<CurriculaResponse>(curricula);
        
        var program = await _programRepository.GetProgramByIdAsync(curricula.program_id);
        if (program != null)
        {
            response.ProgramCode = program.program_code;
            response.ProgramTitle = program.program_title;
        }
        
        var syTerm = await _syTermRepository.GetSyTermByIdAsync(curricula.sy_id);
        if (syTerm != null)
        {
            response.SyYear = syTerm.sy_year;
        }
        
        return response;
    }

    public async Task<CurriculaResponse> CreateCurriculaAsync(CreateCurriculaRequest curriculaRequest)
    {
        var curricula = Curricula.Create(
            curriculaRequest.ProgramCode,
            curriculaRequest.Version,
            curriculaRequest.ProgramId,
            curriculaRequest.SyId,
            curriculaRequest.EffectiveDate,
            curriculaRequest.CurriculumStatus
        );
        var result = await _curriculaRepository.CreateCurriculaAsync(curricula);
        return await GetCurriculaByIdAsync(result.id) ?? _mapper.Map<CurriculaResponse>(result);
    }

    public async Task<CurriculaResponse> UpdateCurriculaAsync(UpdateCurriculaRequest curriculaRequest)
    {
        var curricula = await _curriculaRepository.GetCurriculaByIdAsync(curriculaRequest.Id);

        if (curricula == null)
        {
            throw new Exception("Curriculum not found");
        }

        var curriculumCode = ResolveCurriculumCode(curriculaRequest, curricula);

        curricula.Update(
            curriculumCode,
            curriculaRequest.Version,
            curriculaRequest.ProgramId,
            curriculaRequest.SyId,
            curriculaRequest.EffectiveDate,
            curriculaRequest.CurriculumStatus
        );

        var result = await _curriculaRepository.UpdateCurriculaAsync(curricula);
        return await GetCurriculaByIdAsync(result.id) ?? _mapper.Map<CurriculaResponse>(result);
    }

    public async Task DeleteCurriculaAsync(long id)
    {
        var curricula = await _curriculaRepository.GetCurriculaByIdAsync(id);
        if (curricula == null)
        {
            throw new Exception("Curriculum not found");
        }
        await _curriculaRepository.DeleteCurriculaAsync(curricula);
    }

    private static string ResolveCurriculumCode(UpdateCurriculaRequest request, Curricula existing)
    {
        var fromRequest = request.CurriculumCode?.Trim();
        if (!string.IsNullOrEmpty(fromRequest))
        {
            return fromRequest;
        }

        var fromEntity = existing.curriculum_code?.Trim();
        if (!string.IsNullOrEmpty(fromEntity))
        {
            return fromEntity;
        }

        var programCode = request.ProgramCode?.Trim();
        var version = request.Version?.Trim();
        if (!string.IsNullOrEmpty(programCode) && !string.IsNullOrEmpty(version))
        {
            return $"{programCode}-{version}";
        }

        return string.Empty;
    }
}


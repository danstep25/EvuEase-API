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
    private readonly ICurriculumSupportingDocumentStore _supportingDocumentStore;
    private readonly IMapper _mapper;

    public CurriculaService(
        ICurriculaRepository curriculaRepository,
        IProgramRepository programRepository,
        ISyTermRepository syTermRepository,
        ICurriculumSupportingDocumentStore supportingDocumentStore,
        IMapper mapper)
    {
        _curriculaRepository = curriculaRepository;
        _programRepository = programRepository;
        _syTermRepository = syTermRepository;
        _supportingDocumentStore = supportingDocumentStore;
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

    public async Task<CurriculaResponse?> UploadSupportingDocumentAsync(
        string curriculumCode,
        Stream content,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = curriculumCode.Trim();
        if (string.IsNullOrWhiteSpace(normalizedCode))
        {
            throw new ArgumentException("Curriculum code is required.");
        }

        var curricula = await _curriculaRepository.GetCurriculaByCodeAsync(normalizedCode);
        if (curricula == null)
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(curricula.supporting_document_storage_key))
        {
            await _supportingDocumentStore.DeleteAsync(curricula.supporting_document_storage_key, cancellationToken);
        }

        var (storageKey, safeFileName) = await _supportingDocumentStore.SaveAsync(
            curricula.id,
            content,
            fileName,
            cancellationToken);

        curricula.SetSupportingDocument(safeFileName, storageKey);
        await _curriculaRepository.UpdateCurriculaAsync(curricula);

        return await GetCurriculaByIdAsync(curricula.id);
    }

    public async Task<(Stream Stream, string FileName, string ContentType)?> GetSupportingDocumentAsync(
        string curriculumCode,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = curriculumCode.Trim();
        if (string.IsNullOrWhiteSpace(normalizedCode))
        {
            return null;
        }

        var curricula = await _curriculaRepository.GetCurriculaByCodeAsync(normalizedCode);
        if (curricula == null ||
            string.IsNullOrWhiteSpace(curricula.supporting_document_storage_key) ||
            string.IsNullOrWhiteSpace(curricula.supporting_document_file_name))
        {
            return null;
        }

        var stream = await _supportingDocumentStore.OpenAsync(curricula.supporting_document_storage_key, cancellationToken);
        if (stream == null)
        {
            return null;
        }

        var contentType = ResolveContentType(curricula.supporting_document_file_name);
        return (stream, curricula.supporting_document_file_name, contentType);
    }

    private static string ResolveContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".pdf" => "application/pdf",
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".webp" => "image/webp",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            _ => "application/octet-stream"
        };
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


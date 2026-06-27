using AutoMapper;
using EvuEase.Application.Common;
using EvuEase.Application.DTOs.SyTerm;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Application.Interfaces.Services;
using EvuEase.Domain.Entities;

namespace EvuEase.Application.Services;

public class SyTermService : ISyTermService
{
    private readonly ISyTermRepository _syTermRepository;
    private readonly IMapper _mapper;

    public SyTermService(ISyTermRepository syTermRepository, IMapper mapper)
    {
        _syTermRepository = syTermRepository;
        _mapper = mapper;
    }

    public async Task<PagedResults<SyTermResponse>> GetAllSyTerms(SyTermRequest syTermRequest)
    {
        var pagedEntities = await _syTermRepository.GetAllSyTerms(syTermRequest);
        return pagedEntities.MapToDto<SyTerm, SyTermResponse>(_mapper);
    }

    public async Task<SyTermResponse?> GetSyTermByIdAsync(long id)
    {
        var syTerm = await _syTermRepository.GetSyTermByIdAsync(id);
        return syTerm == null ? null : _mapper.Map<SyTermResponse>(syTerm);
    }

    public async Task<SyTermResponse?> GetCurrentSyTermAsync(CancellationToken cancellationToken = default)
    {
        var syTerm = await _syTermRepository.GetCurrentSyTermAsync(cancellationToken);
        return syTerm == null ? null : _mapper.Map<SyTermResponse>(syTerm);
    }

    public async Task<SyTermResponse> SetCurrentSyTermAsync(long syTermId, CancellationToken cancellationToken = default)
    {
        var result = await _syTermRepository.SetCurrentSyTermAsync(syTermId, cancellationToken);
        return _mapper.Map<SyTermResponse>(result);
    }

    public async Task<SyTermResponse> CreateSyTermAsync(CreateSyTermRequest syTermRequest)
    {
        var syTerm = SyTerm.Create(
            syTermRequest.SyCode,
            syTermRequest.SyYear,
            syTermRequest.SySemester,
            syTermRequest.SyStartDate,
            syTermRequest.SyEndDate,
            syTermRequest.SyEnrollmentStart,
            syTermRequest.SyEnrollmentEnd,
            syTermRequest.SyStatus
        );
        var result = await _syTermRepository.CreateSyTermAsync(syTerm);

        if (IsActiveStatus(syTermRequest.SyStatus))
        {
            result = await _syTermRepository.SetCurrentSyTermAsync(result.sy_id);
        }

        return _mapper.Map<SyTermResponse>(result);
    }

    public async Task<SyTermResponse> UpdateSyTermAsync(UpdateSyTermRequest syTermRequest)
    {
        var syTerm = await _syTermRepository.GetSyTermByIdAsync(syTermRequest.SyId);

        if (syTerm == null)
        {
            throw new Exception("School Year Term not found");
        }

        syTerm.Update(
            syTermRequest.SyCode,
            syTermRequest.SyYear,
            syTermRequest.SySemester,
            syTermRequest.SyStartDate,
            syTermRequest.SyEndDate,
            syTermRequest.SyEnrollmentStart,
            syTermRequest.SyEnrollmentEnd,
            syTermRequest.SyStatus
        );

        var result = await _syTermRepository.UpdateSyTermAsync(syTerm);

        if (IsActiveStatus(syTermRequest.SyStatus))
        {
            result = await _syTermRepository.SetCurrentSyTermAsync(syTermRequest.SyId);
        }

        return _mapper.Map<SyTermResponse>(result);
    }

    private static bool IsActiveStatus(string status) =>
        string.Equals(status, "Active", StringComparison.OrdinalIgnoreCase);

    public async Task DeleteSyTermAsync(long id)
    {
        var syTerm = await _syTermRepository.GetSyTermByIdAsync(id);
        if (syTerm == null)
        {
            throw new Exception("School Year Term not found");
        }
        await _syTermRepository.DeleteSyTermAsync(syTerm);
    }
}


using EvuEase.Application.Common;
using EvuEase.Application.DTOs.Downpayment;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Application.Interfaces.Services;
using EvuEase.Domain.Entities;

namespace EvuEase.Application.Services;

public class DownpaymentService : IDownpaymentService
{
    private readonly IDownpaymentRepository _repository;

    public DownpaymentService(IDownpaymentRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResults<DownpaymentResponse>> GetAllAsync(DownpaymentRequest request)
    {
        var paged = await _repository.GetAllAsync(request);
        var list = paged.Result.Select(MapToResponse).ToList();
        return new PagedResults<DownpaymentResponse>(
            paged.PageIndex,
            paged.PageSize,
            paged.TotalRecords,
            paged.TotalEntries,
            list);
    }

    public async Task<DownpaymentResponse?> GetByIdAsync(long id)
    {
        var row = await _repository.GetByIdAsync(id);
        return row == null ? null : MapToResponse(row);
    }

    public async Task<DownpaymentResponse> CreateAsync(CreateDownpaymentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ProgramCode))
        {
            throw new InvalidOperationException("Program code is required.");
        }

        if (await _repository.ExistsOtherActiveWithProgramCodeAsync(request.ProgramCode, excludeId: null))
        {
            throw new InvalidOperationException(
                "A downpayment rule already exists for this program code. Edit the existing rule instead of adding another.");
        }

        var entity = DpPercentage.Create(
            request.ProgramCode,
            request.ProgramTitle,
            request.Batch,
            request.DownpaymentPercent,
            request.EffectiveSchoolYear);
        var saved = await _repository.CreateAsync(entity);
        return MapToResponse(saved);
    }

    public async Task<DownpaymentResponse> UpdateAsync(UpdateDownpaymentRequest request)
    {
        var oldRow = await _repository.GetByIdAsync(request.Id);
        if (oldRow == null)
        {
            throw new Exception($"Downpayment with ID '{request.Id}' was not found.");
        }

        var newCode = !string.IsNullOrWhiteSpace(request.ProgramCode)
            ? request.ProgramCode.Trim()
            : (oldRow.program_code ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(newCode))
        {
            throw new InvalidOperationException("Program code is required.");
        }

        if (await _repository.ExistsOtherActiveWithProgramCodeAsync(newCode, excludeId: request.Id))
        {
            throw new InvalidOperationException(
                "Another active downpayment already uses this program code.");
        }

        var newEntity = DpPercentage.Create(
            newCode,
            !string.IsNullOrWhiteSpace(request.ProgramTitle) ? request.ProgramTitle.Trim() : oldRow.program_title,
            !string.IsNullOrWhiteSpace(request.Batch) ? request.Batch.Trim() : oldRow.batch,
            request.DownpaymentPercent,
            !string.IsNullOrWhiteSpace(request.EffectiveSchoolYear) ? request.EffectiveSchoolYear.Trim() : oldRow.effective_school_year);

        var saved = await _repository.SupersedeWithNewRowAsync(oldRow, newEntity);
        return MapToResponse(saved);
    }

    public async Task DeleteAsync(long id)
    {
        var row = await _repository.GetByIdAsync(id);
        if (row == null)
        {
            throw new Exception($"Downpayment with ID '{id}' was not found.");
        }

        await _repository.DeleteAsync(row);
    }

    public async Task<IReadOnlyList<DownpaymentResponse>> GetHistoryByProgramCodeAsync(string programCode)
    {
        var rows = await _repository.GetHistoryByProgramCodeAsync(programCode);
        return rows.Select(MapToResponse).ToList();
    }

    private static DownpaymentResponse MapToResponse(DpPercentage d)
    {
        return new DownpaymentResponse
        {
            Id = d.id,
            ProgramCode = d.program_code,
            ProgramTitle = d.program_title,
            Batch = d.batch,
            DownpaymentPercent = d.downpayment_percent,
            EffectiveSchoolYear = d.effective_school_year,
            CreatedAt = d.created_at,
            UpdatedAt = d.updated_at,
            CreatedBy = d.created_by,
            UpdatedBy = d.updated_by
        };
    }
}

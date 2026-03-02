using EvuEase.Application.Common;
using EvuEase.Application.DTOs.MiscellaneousFee;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Application.Interfaces.Services;
using EvuEase.Domain.Entities;

namespace EvuEase.Application.Services;

public class MiscellaneousFeeService : IMiscellaneousFeeService
{
    private readonly IMiscellaneousFeeRepository _repository;

    public MiscellaneousFeeService(IMiscellaneousFeeRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResults<MiscellaneousFeeResponse>> GetAllMiscellaneousFees(MiscellaneousFeeRequest request)
    {
        var pagedEntities = await _repository.GetAllMiscellaneousFees(request);
        var list = pagedEntities.Result.ToList();

        var responses = list.Select(MapToResponse).ToList();

        return new PagedResults<MiscellaneousFeeResponse>(
            pagedEntities.PageIndex,
            pagedEntities.PageSize,
            pagedEntities.TotalRecords,
            pagedEntities.TotalEntries,
            responses
        );
    }

    public async Task<MiscellaneousFeeResponse?> GetMiscellaneousFeeByIdAsync(long id)
    {
        var fee = await _repository.GetMiscellaneousFeeByIdAsync(id);
        return fee == null ? null : MapToResponse(fee);
    }

    public async Task<MiscellaneousFeeResponse> CreateMiscellaneousFeeAsync(CreateMiscellaneousFeeRequest request)
    {
        var fee = MiscellaneousFee.Create(
            request.SyId,
            request.Batch,
            request.Semester,
            request.MiscellaneousFee,
            request.Cash,
            request.LowMonthlyPayment
        );

        var result = await _repository.CreateMiscellaneousFeeAsync(fee);
        return MapToResponse(result);
    }

    public async Task<MiscellaneousFeeResponse> UpdateMiscellaneousFeeAsync(UpdateMiscellaneousFeeRequest request)
    {
        var fee = await _repository.GetMiscellaneousFeeByIdAsync(request.Id);
        if (fee == null)
        {
            throw new Exception($"Miscellaneous fee with ID '{request.Id}' not found.");
        }

        fee.Update(
            request.SyId,
            request.Batch,
            request.Semester,
            request.MiscellaneousFee,
            request.Cash,
            request.LowMonthlyPayment
        );

        var result = await _repository.UpdateMiscellaneousFeeAsync(fee);
        return MapToResponse(result);
    }

    public async Task DeleteMiscellaneousFeeAsync(long id)
    {
        var fee = await _repository.GetMiscellaneousFeeByIdAsync(id);
        if (fee == null)
        {
            throw new Exception($"Miscellaneous fee with ID '{id}' not found.");
        }
        await _repository.DeleteMiscellaneousFeeAsync(fee);
    }

    private MiscellaneousFeeResponse MapToResponse(MiscellaneousFee fee)
    {
        return new MiscellaneousFeeResponse
        {
            Id = fee.id,
            SyId = fee.sy_id,
            Batch = fee.batch,
            Semester = fee.semester,
            MiscellaneousFee = fee.miscellaneous_fee,
            Cash = fee.cash,
            LowMonthlyPayment = fee.low_monthly_payment,
            CreatedAt = fee.created_at,
            UpdatedAt = fee.updated_at
        };
    }
}



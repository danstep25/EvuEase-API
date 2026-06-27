using EvuEase.Application.Common;
using EvuEase.Application.DTOs.OtherSchoolFee;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Application.Interfaces.Services;
using EvuEase.Domain.Entities;

namespace EvuEase.Application.Services;

public class OtherSchoolFeeService : IOtherSchoolFeeService
{
    private readonly IOtherSchoolFeeRepository _repository;

    public OtherSchoolFeeService(IOtherSchoolFeeRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResults<OtherSchoolFeeResponse>> GetAllOtherSchoolFees(OtherSchoolFeeRequest request)
    {
        var pagedEntities = await _repository.GetAllOtherSchoolFees(request);
        var list = pagedEntities.Result.ToList();

        var responses = list.Select(MapToResponse).ToList();

        return new PagedResults<OtherSchoolFeeResponse>(
            pagedEntities.PageIndex,
            pagedEntities.PageSize,
            pagedEntities.TotalRecords,
            pagedEntities.TotalEntries,
            responses
        );
    }

    public async Task<OtherSchoolFeeResponse?> GetOtherSchoolFeeByIdAsync(long id)
    {
        var fee = await _repository.GetOtherSchoolFeeByIdAsync(id);
        return fee == null ? null : MapToResponse(fee);
    }

    public async Task<OtherSchoolFeeResponse> CreateOtherSchoolFeeAsync(CreateOtherSchoolFeeRequest request)
    {
        FeeChargeAmountValidation.EnsureFeeAmounts(request.Cash, request.LowMonthlyPayment);

        var fee = OtherSchoolFee.Create(
            request.SyId,
            request.Batch,
            request.Semester,
            request.SchoolFee,
            request.Cash,
            request.LowMonthlyPayment
        );

        var result = await _repository.CreateOtherSchoolFeeAsync(fee);
        return MapToResponse(result);
    }

    public async Task<OtherSchoolFeeResponse> UpdateOtherSchoolFeeAsync(UpdateOtherSchoolFeeRequest request)
    {
        var fee = await _repository.GetOtherSchoolFeeByIdAsync(request.Id);
        if (fee == null)
        {
            throw new Exception($"Other school fee with ID '{request.Id}' not found.");
        }

        FeeChargeAmountValidation.EnsureFeeAmounts(request.Cash, request.LowMonthlyPayment);

        fee.Update(
            request.SyId,
            request.Batch,
            request.Semester,
            request.SchoolFee,
            request.Cash,
            request.LowMonthlyPayment
        );

        var result = await _repository.UpdateOtherSchoolFeeAsync(fee);
        return MapToResponse(result);
    }

    public async Task DeleteOtherSchoolFeeAsync(long id)
    {
        var fee = await _repository.GetOtherSchoolFeeByIdAsync(id);
        if (fee == null)
        {
            throw new Exception($"Other school fee with ID '{id}' not found.");
        }
        await _repository.DeleteOtherSchoolFeeAsync(fee);
    }

    private OtherSchoolFeeResponse MapToResponse(OtherSchoolFee fee)
    {
        return new OtherSchoolFeeResponse
        {
            Id = fee.id,
            SyId = fee.sy_id,
            Batch = fee.batch,
            Semester = fee.semester,
            SchoolFee = fee.school_fee,
            Cash = fee.cash,
            LowMonthlyPayment = fee.low_monthly_payment,
            CreatedAt = fee.created_at,
            UpdatedAt = fee.updated_at
        };
    }
}




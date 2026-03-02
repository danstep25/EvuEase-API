using EvuEase.Application.Common;
using EvuEase.Application.DTOs.TuitionFee;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Application.Interfaces.Services;
using EvuEase.Domain.Entities;

namespace EvuEase.Application.Services;

public class TuitionFeeService : ITuitionFeeService
{
    private readonly ITuitionFeeRepository _tuitionFeeRepository;

    public TuitionFeeService(ITuitionFeeRepository tuitionFeeRepository)
    {
        _tuitionFeeRepository = tuitionFeeRepository;
    }

    public async Task<PagedResults<TuitionFeeResponse>> GetAllTuitionFees(TuitionFeeRequest tuitionFeeRequest)
    {
        var pagedEntities = await _tuitionFeeRepository.GetAllTuitionFees(tuitionFeeRequest);
        var tuitionFeeList = pagedEntities.Result.ToList();
        
        var responseList = tuitionFeeList.Select(MapToResponse).ToList();
        
        return new PagedResults<TuitionFeeResponse>(
            pagedEntities.PageIndex,
            pagedEntities.PageSize,
            pagedEntities.TotalRecords,
            pagedEntities.TotalEntries,
            responseList
        );
    }

    public async Task<TuitionFeeResponse?> GetTuitionFeeByIdAsync(long id)
    {
        var tuitionFee = await _tuitionFeeRepository.GetTuitionFeeByIdAsync(id);
        if (tuitionFee == null)
        {
            return null;
        }
        
        return MapToResponse(tuitionFee);
    }

    public async Task<TuitionFeeResponse> CreateTuitionFeeAsync(CreateTuitionFeeRequest tuitionFeeRequest)
    {
        var tuitionFee = TuitionFee.Create(
            tuitionFeeRequest.SyId,
            tuitionFeeRequest.Batch,
            tuitionFeeRequest.Semester,
            tuitionFeeRequest.CourseCode,
            tuitionFeeRequest.CourseTitle,
            tuitionFeeRequest.Component,
            tuitionFeeRequest.Units,
            tuitionFeeRequest.Cash,
            tuitionFeeRequest.LowMonthlyPayment
        );

        try
        {
            var result = await _tuitionFeeRepository.CreateTuitionFeeAsync(tuitionFee);
            return MapToResponse(result);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to create tuition fee: {ex.Message}. Inner exception: {ex.InnerException?.Message}", ex);
        }
    }

    public async Task<TuitionFeeResponse> UpdateTuitionFeeAsync(UpdateTuitionFeeRequest tuitionFeeRequest)
    {
        var tuitionFee = await _tuitionFeeRepository.GetTuitionFeeByIdAsync(tuitionFeeRequest.Id);
        if (tuitionFee == null)
        {
            throw new Exception($"Tuition fee with ID '{tuitionFeeRequest.Id}' not found.");
        }

        tuitionFee.Update(
            tuitionFeeRequest.SyId,
            tuitionFeeRequest.Batch,
            tuitionFeeRequest.Semester,
            tuitionFeeRequest.CourseCode,
            tuitionFeeRequest.CourseTitle,
            tuitionFeeRequest.Component,
            tuitionFeeRequest.Units,
            tuitionFeeRequest.Cash,
            tuitionFeeRequest.LowMonthlyPayment
        );

        try
        {
            var result = await _tuitionFeeRepository.UpdateTuitionFeeAsync(tuitionFee);
            return MapToResponse(result);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to update tuition fee: {ex.Message}. Inner exception: {ex.InnerException?.Message}", ex);
        }
    }

    public async Task DeleteTuitionFeeAsync(long id)
    {
        var tuitionFee = await _tuitionFeeRepository.GetTuitionFeeByIdAsync(id);
        if (tuitionFee == null)
        {
            throw new Exception($"Tuition fee with ID '{id}' not found.");
        }
        await _tuitionFeeRepository.DeleteTuitionFeeAsync(tuitionFee);
    }

    private TuitionFeeResponse MapToResponse(TuitionFee tuitionFee)
    {
        return new TuitionFeeResponse
        {
            Id = tuitionFee.id,
            SyId = tuitionFee.sy_id,
            Batch = tuitionFee.batch,
            Semester = tuitionFee.semester,
            CourseCode = tuitionFee.course_code,
            CourseTitle = tuitionFee.course_title,
            Component = tuitionFee.component,
            Units = tuitionFee.units,
            Cash = tuitionFee.cash,
            LowMonthlyPayment = tuitionFee.low_monthly_payment,
            CreatedAt = tuitionFee.created_at,
            UpdatedAt = tuitionFee.updated_at
        };
    }
}



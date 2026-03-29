using EvuEase.Application.Common;
using EvuEase.Application.DTOs.TuitionFee;

namespace EvuEase.Application.Interfaces.Services;

public interface ITuitionFeeService
{
    Task<PagedResults<TuitionFeeResponse>> GetAllTuitionFees(TuitionFeeRequest tuitionFeeRequest);
    Task<TuitionFeeResponse?> GetTuitionFeeByIdAsync(long id);
    Task<TuitionFeeResponse> CreateTuitionFeeAsync(CreateTuitionFeeRequest tuitionFeeRequest);
    Task<TuitionFeeResponse> UpdateTuitionFeeAsync(UpdateTuitionFeeRequest tuitionFeeRequest);
    Task DeleteTuitionFeeAsync(long id);
}




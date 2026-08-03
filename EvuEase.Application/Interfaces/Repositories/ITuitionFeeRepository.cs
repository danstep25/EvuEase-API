using EvuEase.Application.Common;
using EvuEase.Application.DTOs.TuitionFee;
using EvuEase.Domain.Entities;

namespace EvuEase.Application.Interfaces.Repositories;

public interface ITuitionFeeRepository
{
    Task<PagedResults<TuitionFee>> GetAllTuitionFees(TuitionFeeRequest tuitionFeeRequest);
    Task<TuitionFee?> GetTuitionFeeByIdAsync(long id);
    Task<TuitionFee> CreateTuitionFeeAsync(TuitionFee tuitionFee);
    Task<TuitionFee> UpdateTuitionFeeAsync(TuitionFee tuitionFee);
    Task DeleteTuitionFeeAsync(TuitionFee tuitionFee);
}




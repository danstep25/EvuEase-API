using EvuEase.Application.Common;
using EvuEase.Application.DTOs.OtherSchoolFee;
using EvuEase.Domain.Entities;

namespace EvuEase.Application.Interfaces.Repositories;

public interface IOtherSchoolFeeRepository
{
    Task<PagedResults<OtherSchoolFee>> GetAllOtherSchoolFees(OtherSchoolFeeRequest request);
    Task<OtherSchoolFee?> GetOtherSchoolFeeByIdAsync(long id);
    Task<OtherSchoolFee> CreateOtherSchoolFeeAsync(OtherSchoolFee fee);
    Task<OtherSchoolFee> UpdateOtherSchoolFeeAsync(OtherSchoolFee fee);
    Task DeleteOtherSchoolFeeAsync(OtherSchoolFee fee);
}



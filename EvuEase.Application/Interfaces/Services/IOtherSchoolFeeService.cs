using EvuEase.Application.Common;
using EvuEase.Application.DTOs.OtherSchoolFee;

namespace EvuEase.Application.Interfaces.Services;

public interface IOtherSchoolFeeService
{
    Task<PagedResults<OtherSchoolFeeResponse>> GetAllOtherSchoolFees(OtherSchoolFeeRequest request);
    Task<OtherSchoolFeeResponse?> GetOtherSchoolFeeByIdAsync(long id);
    Task<OtherSchoolFeeResponse> CreateOtherSchoolFeeAsync(CreateOtherSchoolFeeRequest request);
    Task<OtherSchoolFeeResponse> UpdateOtherSchoolFeeAsync(UpdateOtherSchoolFeeRequest request);
    Task DeleteOtherSchoolFeeAsync(long id);
}




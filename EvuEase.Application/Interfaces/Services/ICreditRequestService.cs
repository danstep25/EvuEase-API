using EvuEase.Application.Common;
using EvuEase.Application.DTOs.CreditRequest;

namespace EvuEase.Application.Interfaces.Services;

public interface ICreditRequestService
{
    Task<PagedResults<CreditRequestResponse>> GetAllCreditRequestsAsync(
        CreditRequestRequest request,
        CancellationToken cancellationToken = default);

    Task<CreditRequestResponse?> GetCreditRequestByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<CreditRequestResponse> CreateCreditRequestAsync(
        CreateCreditRequestRequest request,
        CancellationToken cancellationToken = default);

    Task<CreditRequestResponse?> UpdateCreditRequestStatusAsync(
        long id,
        UpdateCreditRequestStatusRequest request,
        CancellationToken cancellationToken = default);

    Task<CreditRequestResponse?> UploadSignedPdfAsync(
        long id,
        Stream content,
        string fileName,
        CancellationToken cancellationToken = default);

    Task<(Stream Stream, string FileName)?> GetSignedPdfAsync(long id, CancellationToken cancellationToken = default);

    Task<CreditRequestResponse?> RemoveSignedPdfAsync(long id, CancellationToken cancellationToken = default);
}

using EvuEase.Application.Common;
using EvuEase.Application.DTOs.PaymentScheme;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Application.Interfaces.Services;
using EvuEase.Domain.Entities;

namespace EvuEase.Application.Services;

public class PaymentSchemeService : IPaymentSchemeService
{
    private readonly IPaymentSchemeRepository _repository;

    public PaymentSchemeService(IPaymentSchemeRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResults<PaymentSchemeResponse>> GetAllAsync(PaymentSchemeRequest request)
    {
        var paged = await _repository.GetAllAsync(request);
        var schemes = paged.Result.ToList();
        var schemeIds = schemes.Select(s => s.id).ToList();
        var installments = await _repository.GetInstallmentsBySchemeIdsAsync(schemeIds);
        var byScheme = installments.GroupBy(i => i.payment_scheme_id).ToDictionary(g => g.Key, g => g.ToList());

        var responses = schemes.Select(s => MapToResponse(s, byScheme.GetValueOrDefault(s.id, new List<PaymentSchemeInstallment>()))).ToList();

        return new PagedResults<PaymentSchemeResponse>(
            paged.PageIndex,
            paged.PageSize,
            paged.TotalRecords,
            paged.TotalEntries,
            responses);
    }

    public async Task<PaymentSchemeResponse?> GetByIdAsync(long id)
    {
        var scheme = await _repository.GetByIdAsync(id);
        if (scheme == null)
        {
            return null;
        }

        var installments = await _repository.GetInstallmentsBySchemeIdsAsync(new[] { scheme.id });
        return MapToResponse(scheme, installments);
    }

    public async Task<PaymentSchemeResponse> CreateAsync(CreatePaymentSchemeRequest request)
    {
        ValidateRequest(request.SchoolYear, request.Semester, request.Installments);

        if (await _repository.ExistsOtherActiveAsync(request.SchoolYear!, request.Semester!, null))
        {
            throw new InvalidOperationException("A payment scheme already exists for this school year and semester.");
        }

        var normalized = NormalizeInstallments(request.Installments);
        var description = ResolveDescription(request.Description, normalized.Count);
        var scheme = PaymentScheme.Create(request.SchoolYear, request.Semester, description);
        var installmentEntities = normalized.Select((row, index) =>
            PaymentSchemeInstallment.Create(0, index + 1, row.PaymentName, row.DueDate)).ToList();

        var created = await _repository.CreateAsync(scheme, installmentEntities);
        var savedInstallments = await _repository.GetInstallmentsBySchemeIdsAsync(new[] { created.id });
        return MapToResponse(created, savedInstallments);
    }

    public async Task<PaymentSchemeResponse> UpdateAsync(UpdatePaymentSchemeRequest request)
    {
        ValidateRequest(request.SchoolYear, request.Semester, request.Installments);

        var scheme = await _repository.GetByIdAsync(request.Id);
        if (scheme == null)
        {
            throw new Exception($"Payment scheme with ID '{request.Id}' not found.");
        }

        if (await _repository.ExistsOtherActiveAsync(request.SchoolYear!, request.Semester!, request.Id))
        {
            throw new InvalidOperationException("A payment scheme already exists for this school year and semester.");
        }

        var normalized = NormalizeInstallments(request.Installments);
        var description = ResolveDescription(request.Description, normalized.Count);
        scheme.Update(request.SchoolYear, request.Semester, description);

        var installmentEntities = normalized.Select((row, index) =>
            PaymentSchemeInstallment.Create(scheme.id, index + 1, row.PaymentName, row.DueDate)).ToList();

        var updated = await _repository.UpdateAsync(scheme, installmentEntities);
        var savedInstallments = await _repository.GetInstallmentsBySchemeIdsAsync(new[] { updated.id });
        return MapToResponse(updated, savedInstallments);
    }

    public async Task DeleteAsync(long id)
    {
        var scheme = await _repository.GetByIdAsync(id);
        if (scheme == null)
        {
            throw new Exception($"Payment scheme with ID '{id}' not found.");
        }

        await _repository.DeleteAsync(scheme);
    }

    private static void ValidateRequest(string? schoolYear, string? semester, List<PaymentSchemeInstallmentInputDto> installments)
    {
        if (string.IsNullOrWhiteSpace(schoolYear))
        {
            throw new ArgumentException("School year is required.");
        }

        if (string.IsNullOrWhiteSpace(semester))
        {
            throw new ArgumentException("Semester is required.");
        }

        if (installments == null || installments.Count == 0)
        {
            throw new ArgumentException("At least one installment is required.");
        }

        foreach (var row in installments)
        {
            if (string.IsNullOrWhiteSpace(row.PaymentName))
            {
                throw new ArgumentException("Each installment must have a payment name.");
            }
        }
    }

    private static List<PaymentSchemeInstallmentInputDto> NormalizeInstallments(List<PaymentSchemeInstallmentInputDto> installments)
    {
        return installments
            .Select((row, index) => new PaymentSchemeInstallmentInputDto
            {
                PaymentName = row.PaymentName?.Trim(),
                DueDate = row.DueDate.Date
            })
            .OrderBy(row => row.DueDate)
            .ThenBy(row => row.PaymentName)
            .ToList();
    }

    private static string BuildDescription(int count)
    {
        return $"Payment split into {count} installment{(count == 1 ? string.Empty : "s")}";
    }

    private static string ResolveDescription(string? description, int installmentCount)
    {
        return !string.IsNullOrWhiteSpace(description)
            ? description.Trim()
            : BuildDescription(installmentCount);
    }

    private static PaymentSchemeResponse MapToResponse(PaymentScheme scheme, List<PaymentSchemeInstallment> installments)
    {
        return new PaymentSchemeResponse
        {
            Id = scheme.id,
            SchoolYear = scheme.school_year,
            Semester = scheme.semester,
            Description = scheme.description,
            InstallmentCount = installments.Count,
            Installments = installments
                .OrderBy(i => i.installment_order)
                .Select(i => new PaymentSchemeInstallmentDto
                {
                    Id = i.id,
                    InstallmentOrder = i.installment_order,
                    PaymentName = i.payment_name,
                    DueDate = i.due_date
                })
                .ToList(),
            CreatedAt = scheme.created_at,
            UpdatedAt = scheme.updated_at
        };
    }
}

using EvuEase.Application.DTOs.EvaluationAudit;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Domain.Entities;
using EvuEase.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EvuEase.Infrastructure.Repositories;

public class SubjectEvaluationAuditRepository : ISubjectEvaluationAuditRepository
{
    private readonly AppDbContext _dbContext;

    public SubjectEvaluationAuditRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<SubjectEvaluationAudit>> GetAllAsync(
        EvaluationAuditRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = ApplyFilters(BuildQuery(), request);

            return await query
                .OrderByDescending(e => e.evaluated_at)
                .ThenByDescending(e => e.id)
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex) when (IsMissingAuditTable(ex))
        {
            return Array.Empty<SubjectEvaluationAudit>();
        }
    }

    public async Task<int> CountAllAsync(EvaluationAuditRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            return await ApplyFilters(BuildQuery(), request).CountAsync(cancellationToken);
        }
        catch (SqlException ex) when (ex.Number == 208)
        {
            return 0;
        }
        catch (Exception ex) when (IsMissingAuditTable(ex))
        {
            return 0;
        }
    }

    public async Task<SubjectEvaluationAudit?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await BuildQuery().FirstOrDefaultAsync(e => e.id == id, cancellationToken);
        }
        catch (SqlException ex) when (ex.Number == 208)
        {
            return null;
        }
        catch (Exception ex) when (IsMissingAuditTable(ex))
        {
            return null;
        }
    }

    public async Task<SubjectEvaluationAudit> CreateAsync(
        SubjectEvaluationAudit entity,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbContext.SubjectEvaluationAudits.AddAsync(entity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return entity;
        }
        catch (SqlException ex) when (ex.Number == 208)
        {
            throw new InvalidOperationException(
                "Evaluation audit table is missing. Run migration 20250712000000_AddSubjectEvaluationAudit.sql first.",
                ex);
        }
        catch (Exception ex) when (IsMissingAuditTable(ex))
        {
            throw new InvalidOperationException(
                "Evaluation audit table is missing. Run migration 20250712000000_AddSubjectEvaluationAudit.sql first.",
                ex);
        }
    }

    private static bool IsMissingAuditTable(Exception ex)
    {
        for (var current = ex; current != null; current = current.InnerException)
        {
            if (current is SqlException sql && sql.Number == 208)
            {
                return true;
            }

            if (current.Message.Contains("tbl_subject_evaluation_audit", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private IQueryable<SubjectEvaluationAudit> BuildQuery()
    {
        return _dbContext.SubjectEvaluationAudits.AsNoTracking()
            .Where(e => e.status && e.deleted_at == null);
    }

    private static IQueryable<SubjectEvaluationAudit> ApplyFilters(
        IQueryable<SubjectEvaluationAudit> query,
        EvaluationAuditRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            return query;
        }

        var term = request.SearchTerm.Trim().ToLower();
        return query.Where(e =>
            e.student_number.ToLower().Contains(term)
            || e.student_name.ToLower().Contains(term)
            || e.program_code.ToLower().Contains(term)
            || e.evaluated_by.ToLower().Contains(term)
            || e.school_year_term.ToLower().Contains(term));
    }
}

using EvuEase.Application.DTOs.Archive;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Domain.Entities;
using EvuEase.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EvuEase.Infrastructure.Repositories;

public class ArchiveRepository : IArchiveRepository
{
    private readonly AppDbContext _db;

    public ArchiveRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<ArchivedProgramDto>> ListArchivedProgramsAsync(
        ArchiveProgramListQuery query,
        CancellationToken cancellationToken = default)
    {
        var q = _db.Programs.AsNoTracking().Where(p => !p.status);

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var term = query.SearchTerm.Trim().ToLowerInvariant();
            q = q.Where(p =>
                p.program_code.ToLower().Contains(term) ||
                p.program_title.ToLower().Contains(term));
        }

        if (query.DateFrom.HasValue)
        {
            q = q.Where(p => (p.deleted_at ?? p.updated_at) >= query.DateFrom.Value);
        }

        if (query.DateTo.HasValue)
        {
            var end = query.DateTo.Value.Date.AddDays(1);
            q = q.Where(p => (p.deleted_at ?? p.updated_at) < end);
        }

        if (!string.IsNullOrWhiteSpace(query.DeletedBy))
        {
            var by = query.DeletedBy.Trim();
            q = q.Where(p => p.deleted_by == by);
        }

        if (!string.IsNullOrWhiteSpace(query.YearsOfCompletion)
            && int.TryParse(query.YearsOfCompletion.Trim(), out var years))
        {
            q = q.Where(p => p.program_completionyears == years);
        }

        var rows = await q
            .OrderByDescending(p => p.deleted_at ?? p.updated_at)
            .ToListAsync(cancellationToken);

        var nameMap = await ResolveDeletedByNamesAsync(rows.Select(p => p.deleted_by), cancellationToken);

        return rows.Select(p => new ArchivedProgramDto
        {
            Id = p.program_id.ToString(),
            DeletedAt = (p.deleted_at ?? p.updated_at ?? DateTime.UtcNow).ToString("O"),
            DeletedBy = ResolveName(nameMap, p.deleted_by),
            ProgramCode = p.program_code,
            ProgramTitle = p.program_title,
            Years = p.program_completionyears,
            TotalUnits = p.program_totalunits ?? 0
        }).ToList();
    }

    public async Task<IReadOnlyList<ArchivedStudentDto>> ListArchivedStudentsAsync(
        ArchiveStudentListQuery query,
        CancellationToken cancellationToken = default)
    {
        var q = _db.Students.AsNoTracking().Where(s => !s.status);

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var term = query.SearchTerm.Trim().ToLowerInvariant();
            q = q.Where(s =>
                s.student_number.ToLower().Contains(term) ||
                s.first_name.ToLower().Contains(term) ||
                s.last_name.ToLower().Contains(term) ||
                (s.middle_name != null && s.middle_name.ToLower().Contains(term)));
        }

        if (query.DateFrom.HasValue)
        {
            q = q.Where(s => (s.deleted_at ?? s.updated_at) >= query.DateFrom.Value);
        }

        if (query.DateTo.HasValue)
        {
            var end = query.DateTo.Value.Date.AddDays(1);
            q = q.Where(s => (s.deleted_at ?? s.updated_at) < end);
        }

        if (!string.IsNullOrWhiteSpace(query.DeletedBy))
        {
            var by = query.DeletedBy.Trim();
            q = q.Where(s => s.deleted_by == by);
        }

        if (!string.IsNullOrWhiteSpace(query.ProgramCode))
        {
            var code = query.ProgramCode.Trim().ToLowerInvariant();
            q = q.Where(s => s.program_code.ToLower() == code);
        }

        if (!string.IsNullOrWhiteSpace(query.YearLevel))
        {
            var level = query.YearLevel.Trim().ToLowerInvariant();
            q = q.Where(s => s.year_level.ToLower() == level);
        }

        var rows = await q
            .OrderByDescending(s => s.deleted_at ?? s.updated_at)
            .ToListAsync(cancellationToken);

        var nameMap = await ResolveDeletedByNamesAsync(rows.Select(s => s.deleted_by), cancellationToken);

        return rows.Select(s => new ArchivedStudentDto
        {
            Id = s.id.ToString(),
            DeletedAt = (s.deleted_at ?? s.updated_at ?? DateTime.UtcNow).ToString("O"),
            DeletedBy = ResolveName(nameMap, s.deleted_by),
            StudentNumber = s.student_number,
            DisplayName = FormatStudentName(s),
            ProgramCode = s.program_code
        }).ToList();
    }

    public async Task<IReadOnlyList<ArchivedSchoolYearDto>> ListArchivedSchoolYearsAsync(
        ArchiveSchoolYearListQuery query,
        CancellationToken cancellationToken = default)
    {
        var q = _db.SyTerms.AsNoTracking().Where(s => !s.status);

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var term = query.SearchTerm.Trim().ToLowerInvariant();
            q = q.Where(s =>
                s.sy_code.ToLower().Contains(term) ||
                s.sy_year.ToLower().Contains(term) ||
                s.sy_semester.ToLower().Contains(term));
        }

        if (query.DateFrom.HasValue)
        {
            q = q.Where(s => (s.deleted_at ?? s.updated_at) >= query.DateFrom.Value);
        }

        if (query.DateTo.HasValue)
        {
            var end = query.DateTo.Value.Date.AddDays(1);
            q = q.Where(s => (s.deleted_at ?? s.updated_at) < end);
        }

        if (!string.IsNullOrWhiteSpace(query.DeletedBy))
        {
            var by = query.DeletedBy.Trim();
            q = q.Where(s => s.deleted_by == by);
        }

        if (!string.IsNullOrWhiteSpace(query.Semester))
        {
            var sem = query.Semester.Trim();
            q = q.Where(s => s.sy_semester == sem);
        }

        var rows = await q
            .OrderByDescending(s => s.deleted_at ?? s.updated_at)
            .ToListAsync(cancellationToken);

        var nameMap = await ResolveDeletedByNamesAsync(rows.Select(s => s.deleted_by), cancellationToken);

        return rows.Select(s => new ArchivedSchoolYearDto
        {
            Id = s.sy_id.ToString(),
            DeletedAt = (s.deleted_at ?? s.updated_at ?? DateTime.UtcNow).ToString("O"),
            DeletedBy = ResolveName(nameMap, s.deleted_by),
            Label = $"{s.sy_year} {s.sy_semester}".Trim()
        }).ToList();
    }

    public async Task<IReadOnlyList<ArchivedCurriculumDto>> ListArchivedCurriculaAsync(
        ArchiveListQuery query,
        CancellationToken cancellationToken = default)
    {
        var q = _db.Curricula.AsNoTracking().Where(c => !c.status);

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var term = query.SearchTerm.Trim().ToLowerInvariant();
            q = q.Where(c =>
                c.curriculum_code.ToLower().Contains(term) ||
                c.version.ToLower().Contains(term));
        }

        if (query.DateFrom.HasValue)
        {
            q = q.Where(c => (c.deleted_at ?? c.updated_at) >= query.DateFrom.Value);
        }

        if (query.DateTo.HasValue)
        {
            var end = query.DateTo.Value.Date.AddDays(1);
            q = q.Where(c => (c.deleted_at ?? c.updated_at) < end);
        }

        if (!string.IsNullOrWhiteSpace(query.DeletedBy))
        {
            var by = query.DeletedBy.Trim();
            q = q.Where(c => c.deleted_by == by);
        }

        var rows = await q
            .OrderByDescending(c => c.deleted_at ?? c.updated_at)
            .ToListAsync(cancellationToken);

        var nameMap = await ResolveDeletedByNamesAsync(rows.Select(c => c.deleted_by), cancellationToken);

        return rows.Select(c => new ArchivedCurriculumDto
        {
            Id = c.id.ToString(),
            DeletedAt = (c.deleted_at ?? c.updated_at ?? DateTime.UtcNow).ToString("O"),
            DeletedBy = ResolveName(nameMap, c.deleted_by),
            CurriculumCode = c.curriculum_code,
            CurriculumTitle = $"{c.curriculum_code} (v{c.version})"
        }).ToList();
    }

    public async Task RestoreProgramAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Programs.FirstOrDefaultAsync(p => p.program_id == id, cancellationToken);
        if (entity == null || entity.status)
        {
            throw new KeyNotFoundException($"Archived program {id} was not found.");
        }

        RestoreEntity(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task RestoreStudentAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Students.FirstOrDefaultAsync(s => s.id == id, cancellationToken);
        if (entity == null || entity.status)
        {
            throw new KeyNotFoundException($"Archived student {id} was not found.");
        }

        RestoreEntity(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task RestoreSchoolYearAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.SyTerms.FirstOrDefaultAsync(s => s.sy_id == id, cancellationToken);
        if (entity == null || entity.status)
        {
            throw new KeyNotFoundException($"Archived school year {id} was not found.");
        }

        RestoreEntity(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task RestoreCurriculumAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Curricula.FirstOrDefaultAsync(c => c.id == id, cancellationToken);
        if (entity == null || entity.status)
        {
            throw new KeyNotFoundException($"Archived curriculum {id} was not found.");
        }

        RestoreEntity(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task PermanentDeleteProgramAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Programs.FirstOrDefaultAsync(p => p.program_id == id, cancellationToken);
        if (entity == null || entity.status)
        {
            throw new KeyNotFoundException($"Archived program {id} was not found.");
        }

        _db.Programs.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task PermanentDeleteStudentAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Students.FirstOrDefaultAsync(s => s.id == id, cancellationToken);
        if (entity == null || entity.status)
        {
            throw new KeyNotFoundException($"Archived student {id} was not found.");
        }

        _db.Students.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task PermanentDeleteSchoolYearAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.SyTerms.FirstOrDefaultAsync(s => s.sy_id == id, cancellationToken);
        if (entity == null || entity.status)
        {
            throw new KeyNotFoundException($"Archived school year {id} was not found.");
        }

        _db.SyTerms.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task PermanentDeleteCurriculumAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Curricula.FirstOrDefaultAsync(c => c.id == id, cancellationToken);
        if (entity == null || entity.status)
        {
            throw new KeyNotFoundException($"Archived curriculum {id} was not found.");
        }

        _db.Curricula.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }

    private static void RestoreEntity(BaseEntity entity)
    {
        var type = typeof(BaseEntity);
        type.GetProperty(nameof(BaseEntity.status))?.SetValue(entity, true);
        type.GetProperty(nameof(BaseEntity.updated_at))?.SetValue(entity, DateTime.UtcNow);
        type.GetProperty(nameof(BaseEntity.deleted_at))?.SetValue(entity, null);
        type.GetProperty(nameof(BaseEntity.deleted_by))?.SetValue(entity, null);
    }

    private static string FormatStudentName(Student s)
    {
        var parts = new List<string> { s.last_name + ", " + s.first_name };
        if (!string.IsNullOrWhiteSpace(s.middle_name))
        {
            parts.Add(s.middle_name!);
        }

        return string.Join(" ", parts);
    }

    private async Task<Dictionary<string, string>> ResolveDeletedByNamesAsync(
        IEnumerable<string?> keys,
        CancellationToken cancellationToken)
    {
        var ids = new HashSet<long>();
        foreach (var k in keys)
        {
            if (!string.IsNullOrWhiteSpace(k) && long.TryParse(k.Trim(), out var id))
            {
                ids.Add(id);
            }
        }

        if (ids.Count == 0)
        {
            return new Dictionary<string, string>(StringComparer.Ordinal);
        }

        var users = await _db.Users.AsNoTracking()
            .Where(u => ids.Contains(u.id))
            .Select(u => new { u.id, u.name })
            .ToListAsync(cancellationToken);

        return users.ToDictionary(u => u.id.ToString(), u => u.name, StringComparer.Ordinal);
    }

    private static string ResolveName(Dictionary<string, string> map, string? deletedByKey)
    {
        if (string.IsNullOrWhiteSpace(deletedByKey))
        {
            return string.Empty;
        }

        var key = deletedByKey.Trim();
        return map.TryGetValue(key, out var name) ? name : key;
    }
}

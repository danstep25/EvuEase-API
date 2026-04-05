using System.Collections.Generic;
using EvuEase.Application.Common;
using EvuEase.Application.DTOs.Student;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Domain.Entities;
using EvuEase.Infrastructure.Common;
using EvuEase.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EvuEase.Infrastructure.Repositories;

public class StudentRepository : BaseRepository<Student>, IStudentRepository
{
    public StudentRepository(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        : base(dbContext, httpContextAccessor) { }

    public async Task<PagedResults<Student>> GetAllStudents(StudentRequest request)
    {
        var query = GetAll();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.ToLower();
            query = query.Where(s =>
                s.student_number.ToLower().Contains(term) ||
                s.first_name.ToLower().Contains(term) ||
                s.last_name.ToLower().Contains(term) ||
                (s.middle_name != null && s.middle_name.ToLower().Contains(term)) ||
                (s.email != null && s.email.ToLower().Contains(term)));
        }

        if (!string.IsNullOrWhiteSpace(request.ProgramCode))
        {
            var code = request.ProgramCode.ToLower();
            query = query.Where(s => s.program_code.ToLower().Contains(code));
        }

        if (!string.IsNullOrWhiteSpace(request.YearLevel))
        {
            var level = request.YearLevel.ToLower();
            query = query.Where(s => s.year_level.ToLower() == level);
        }

        if (!string.IsNullOrWhiteSpace(request.Type))
        {
            var t = request.Type.ToLower();
            query = query.Where(s => s.student_type.ToLower() == t);
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            var st = request.Status.ToLower();
            query = query.Where(s => s.enrollment_status.ToLower() == st);
        }

        return await query.PaginateAsync(
            request.PageIndex,
            request.PageSize,
            request.SortKey,
            request.SortDirection
        );
    }

    public async Task<Student?> GetStudentByIdAsync(long id)
    {
        return await GetAll().FirstOrDefaultAsync(s => s.id == id);
    }

    public async Task<Student?> GetStudentByStudentNumberAsync(string studentNumber, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(studentNumber))
        {
            return null;
        }

        var key = studentNumber.Trim();
        return await GetAll()
            .FirstOrDefaultAsync(s => s.student_number == key, cancellationToken);
    }

    public async Task<IReadOnlyDictionary<string, Student>> GetActiveStudentsByStudentNumbersAsync(
        IReadOnlyCollection<string> studentNumbers,
        CancellationToken cancellationToken = default)
    {
        if (studentNumbers == null || studentNumbers.Count == 0)
        {
            return new Dictionary<string, Student>(StringComparer.Ordinal);
        }

        var keys = studentNumbers
            .Select(n => n.Trim())
            .Where(n => n.Length > 0)
            .Distinct(StringComparer.Ordinal)
            .ToList();

        if (keys.Count == 0)
        {
            return new Dictionary<string, Student>(StringComparer.Ordinal);
        }

        var students = await GetAll()
            .Where(s => keys.Contains(s.student_number))
            .ToListAsync(cancellationToken);

        var dict = new Dictionary<string, Student>(StringComparer.Ordinal);
        foreach (var s in students)
        {
            if (!dict.ContainsKey(s.student_number))
            {
                dict[s.student_number] = s;
            }
        }

        return dict;
    }

    public async Task<Student> CreateStudentAsync(Student student)
    {
        await AddAsync(student);
        await SaveChangesAsync();
        return student;
    }

    public async Task<Student> UpdateStudentAsync(Student student)
    {
        await UpdateAsync(student);
        await SaveChangesAsync();
        return student;
    }

    public async Task DeleteStudentAsync(Student student)
    {
        await SoftDeleteAsync(student);
        await SaveChangesAsync();
    }

    public async Task<bool> StudentNumberExistsAsync(string studentNumber, long? excludeId = null)
    {
        var q = GetAll().Where(s => s.student_number == studentNumber);
        if (excludeId.HasValue)
        {
            q = q.Where(s => s.id != excludeId.Value);
        }
        return await q.AnyAsync();
    }

    public async Task<int> CountActiveByProgramCodeAsync(string programCode, CancellationToken cancellationToken = default)
    {
        var code = programCode.Trim();
        return await GetAll()
            .Where(s => s.program_code.ToLower() == code.ToLower())
            .CountAsync(cancellationToken);
    }
}

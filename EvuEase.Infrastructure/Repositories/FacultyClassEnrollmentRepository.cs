using EvuEase.Application.ClassRoster;
using EvuEase.Application.DTOs.ClassRoster;
using EvuEase.Application.DTOs.Student;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Domain.Entities;
using EvuEase.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EvuEase.Infrastructure.Repositories;

public class FacultyClassEnrollmentRepository : IFacultyClassEnrollmentRepository
{
    private readonly AppDbContext _dbContext;

    public FacultyClassEnrollmentRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<StudentClassEnrollmentRowDto>> GetEnrollmentRowsForStudentAsync(
        long studentId,
        CancellationToken cancellationToken = default)
    {
        var raw = await (
                from e in _dbContext.FacultyClassEnrollments.AsNoTracking()
                join s in _dbContext.Students.AsNoTracking() on e.student_id equals s.id
                join fc in _dbContext.FacultyClasses.AsNoTracking() on e.faculty_class_id equals fc.id
                join prog in _dbContext.Programs.AsNoTracking() on fc.program_code equals prog.program_code
                where s.id == studentId
                      && s.status && s.deleted_at == null
                      && e.status && e.deleted_at == null
                      && fc.status && fc.deleted_at == null
                      && prog.status && prog.deleted_at == null
                let units = _dbContext.Courses.AsNoTracking()
                    .Where(c => c.course_code == fc.course_code
                                && c.program_id == prog.program_id
                                && c.status
                                && c.deleted_at == null)
                    .Select(c => (int?)c.course_total_units)
                    .FirstOrDefault() ?? 0
                orderby fc.academic_term descending, fc.course_code
                select new
                {
                    EnrollmentId = e.id,
                    FacultyClassId = fc.id,
                    e.official_grade,
                    e.remarks,
                    fc.course_code,
                    fc.course_title,
                    fc.class_number,
                    fc.section,
                    fc.component,
                    fc.academic_term,
                    fc.program_code,
                    fc.year_level,
                    units
                })
            .ToListAsync(cancellationToken);

        return raw
            .Select(x => new StudentClassEnrollmentRowDto
            {
                EnrollmentId = x.EnrollmentId,
                FacultyClassId = x.FacultyClassId,
                CourseCode = x.course_code,
                CourseTitle = x.course_title,
                ClassNumber = x.class_number,
                Section = x.section,
                Component = x.component,
                AcademicTerm = x.academic_term,
                ProgramCode = x.program_code,
                YearLevel = x.year_level,
                Units = x.units,
                OfficialGrade = x.official_grade,
                Remarks = GradeRosterRemarksHelper.EffectiveRemarks(x.remarks, x.official_grade)
            })
            .ToList();
    }

    public async Task<IReadOnlyDictionary<long, IReadOnlyList<StudentClassEnrollmentRowDto>>> GetEnrollmentRowsForStudentsAsync(
        IReadOnlyList<long> studentIds,
        CancellationToken cancellationToken = default)
    {
        if (studentIds.Count == 0)
        {
            return new Dictionary<long, IReadOnlyList<StudentClassEnrollmentRowDto>>();
        }

        var idSet = studentIds.Distinct().ToList();

        var raw = await (
                from e in _dbContext.FacultyClassEnrollments.AsNoTracking()
                join s in _dbContext.Students.AsNoTracking() on e.student_id equals s.id
                join fc in _dbContext.FacultyClasses.AsNoTracking() on e.faculty_class_id equals fc.id
                join prog in _dbContext.Programs.AsNoTracking() on fc.program_code equals prog.program_code
                where idSet.Contains(s.id)
                      && s.status && s.deleted_at == null
                      && e.status && e.deleted_at == null
                      && fc.status && fc.deleted_at == null
                      && prog.status && prog.deleted_at == null
                let units = _dbContext.Courses.AsNoTracking()
                    .Where(c => c.course_code == fc.course_code
                                && c.program_id == prog.program_id
                                && c.status
                                && c.deleted_at == null)
                    .Select(c => (int?)c.course_total_units)
                    .FirstOrDefault() ?? 0
                orderby s.id, fc.academic_term descending, fc.course_code
                select new
                {
                    s.id,
                    EnrollmentId = e.id,
                    FacultyClassId = fc.id,
                    e.official_grade,
                    e.remarks,
                    fc.course_code,
                    fc.course_title,
                    fc.class_number,
                    fc.section,
                    fc.component,
                    fc.academic_term,
                    fc.program_code,
                    fc.year_level,
                    units
                })
            .ToListAsync(cancellationToken);

        return raw
            .GroupBy(x => x.id)
            .ToDictionary(
                g => g.Key,
                g => (IReadOnlyList<StudentClassEnrollmentRowDto>)g
                    .Select(x => new StudentClassEnrollmentRowDto
                    {
                        EnrollmentId = x.EnrollmentId,
                        FacultyClassId = x.FacultyClassId,
                        CourseCode = x.course_code,
                        CourseTitle = x.course_title,
                        ClassNumber = x.class_number,
                        Section = x.section,
                        Component = x.component,
                        AcademicTerm = x.academic_term,
                        ProgramCode = x.program_code,
                        YearLevel = x.year_level,
                        Units = x.units,
                        OfficialGrade = x.official_grade,
                        Remarks = GradeRosterRemarksHelper.EffectiveRemarks(x.remarks, x.official_grade)
                    })
                    .ToList());
    }

    public async Task<IReadOnlyList<ClassRosterStudentResponse>> GetStudentRowsForClassAsync(
        long facultyClassId,
        CancellationToken cancellationToken = default)
    {
        var rows = await (
                from e in _dbContext.FacultyClassEnrollments.AsNoTracking()
                join s in _dbContext.Students.AsNoTracking() on e.student_id equals s.id
                where e.faculty_class_id == facultyClassId && e.status && e.deleted_at == null && s.status
                orderby s.student_number
                select new { e.id, e.official_grade, e.remarks, s })
            .ToListAsync(cancellationToken);

        return rows
            .Select(x => MapStudentResponse(x.id, x.official_grade, x.remarks, x.s))
            .ToList();
    }

    private static ClassRosterStudentResponse MapStudentResponse(
        long enrollmentId,
        string? officialGrade,
        string? remarks,
        Student student)
    {
        return new ClassRosterStudentResponse
        {
            Id = enrollmentId,
            StudentRecordId = student.id,
            StudentId = student.student_number,
            DisplayName = FormatStudentDisplayName(student),
            ProgramCode = student.program_code,
            YearLevel = student.year_level,
            CurriculumCode = string.IsNullOrWhiteSpace(student.curriculum_code) ? null : student.curriculum_code.Trim(),
            OfficialGrade = officialGrade,
            Remarks = GradeRosterRemarksHelper.EffectiveRemarks(remarks, officialGrade)
        };
    }

    private static string FormatStudentDisplayName(Student s)
    {
        var middle = string.IsNullOrWhiteSpace(s.middle_name) ? string.Empty : " " + s.middle_name.Trim();
        return $"{s.last_name.Trim()}, {s.first_name.Trim()}{middle}".Trim();
    }

    public async Task<ClassRosterStudentResponse?> UpdateOfficialGradeAsync(
        long enrollmentId,
        string? officialGrade,
        string? storedRemarks,
        CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.FacultyClassEnrollments
            .FirstOrDefaultAsync(
                x => x.id == enrollmentId && x.status && x.deleted_at == null,
                cancellationToken);
        if (entity == null)
        {
            return null;
        }

        entity.SetOfficialGrade(officialGrade, storedRemarks);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return await GetStudentRowByEnrollmentIdAsync(enrollmentId, cancellationToken);
    }

    public async Task<int> CountEnrollmentsForClassAsync(long facultyClassId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.FacultyClassEnrollments
            .Where(e => e.faculty_class_id == facultyClassId && e.status && e.deleted_at == null)
            .CountAsync(cancellationToken);
    }

    public async Task<ClassRosterStudentResponse?> AddEnrollmentAsync(
        long facultyClassId,
        long studentId,
        CancellationToken cancellationToken = default)
    {
        var duplicate = await _dbContext.FacultyClassEnrollments.AsNoTracking()
            .AnyAsync(
                e => e.faculty_class_id == facultyClassId && e.student_id == studentId && e.status && e.deleted_at == null,
                cancellationToken);
        if (duplicate)
        {
            return null;
        }

        var entity = FacultyClassEnrollment.Create(facultyClassId, studentId);
        _dbContext.FacultyClassEnrollments.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return await GetStudentRowByEnrollmentIdAsync(entity.id, cancellationToken);
    }

    public async Task<bool> StudentHasCourseEnrollmentAsync(
        long studentId,
        string courseCode,
        CancellationToken cancellationToken = default)
    {
        var normalized = courseCode.Trim().ToLower();
        if (string.IsNullOrEmpty(normalized))
        {
            return false;
        }

        return await (
            from e in _dbContext.FacultyClassEnrollments.AsNoTracking()
            join fc in _dbContext.FacultyClasses.AsNoTracking() on e.faculty_class_id equals fc.id
            where e.student_id == studentId
                  && e.status
                  && e.deleted_at == null
                  && fc.status
                  && fc.deleted_at == null
                  && fc.course_code.ToLower() == normalized
            select e.id
        ).AnyAsync(cancellationToken);
    }

    public async Task<bool> RemoveEnrollmentAsync(
        long facultyClassId,
        long enrollmentId,
        CancellationToken cancellationToken = default)
    {
        var n = await _dbContext.FacultyClassEnrollments
            .Where(e => e.id == enrollmentId && e.faculty_class_id == facultyClassId && e.status && e.deleted_at == null)
            .ExecuteDeleteAsync(cancellationToken);
        return n > 0;
    }

    private async Task<ClassRosterStudentResponse?> GetStudentRowByEnrollmentIdAsync(
        long enrollmentId,
        CancellationToken cancellationToken = default)
    {
        var row = await (
                from e in _dbContext.FacultyClassEnrollments.AsNoTracking()
                join s in _dbContext.Students.AsNoTracking() on e.student_id equals s.id
                where e.id == enrollmentId && e.status && e.deleted_at == null && s.status
                select new { e.id, e.official_grade, e.remarks, s })
            .FirstOrDefaultAsync(cancellationToken);
        if (row == null)
        {
            return null;
        }

        return MapStudentResponse(row.id, row.official_grade, row.remarks, row.s);
    }

    public async Task ReplaceAllForClassAsync(
        long facultyClassId,
        IReadOnlyList<FacultyClassEnrollment> enrollments,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.FacultyClassEnrollments
            .Where(e => e.faculty_class_id == facultyClassId)
            .ExecuteDeleteAsync(cancellationToken);

        if (enrollments.Count > 0)
        {
            await _dbContext.FacultyClassEnrollments.AddRangeAsync(enrollments, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}

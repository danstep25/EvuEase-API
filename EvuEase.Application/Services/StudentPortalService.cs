using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EvuEase.Application.Common;
using EvuEase.Application.DTOs.Course;
using EvuEase.Application.DTOs.Student;
using EvuEase.Application.DTOs.StudentPortal;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Application.Interfaces.Services;
using EvuEase.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace EvuEase.Application.Services;

public class StudentPortalService : IStudentPortalService
{
    private readonly IStudentRepository _studentRepository;
    private readonly IStudentService _studentService;
    private readonly ICourseRepository _courseRepository;
    private readonly IStudentPortalPasswordResetRepository _passwordResetRepository;
    private readonly IConfiguration _configuration;

    public StudentPortalService(
        IStudentRepository studentRepository,
        IStudentService studentService,
        ICourseRepository courseRepository,
        IStudentPortalPasswordResetRepository passwordResetRepository,
        IConfiguration configuration)
    {
        _studentRepository = studentRepository;
        _studentService = studentService;
        _courseRepository = courseRepository;
        _passwordResetRepository = passwordResetRepository;
        _configuration = configuration;
    }

    public async Task<StudentPortalAuthResponse> LoginAsync(
        StudentPortalLoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var studentNumber = request.StudentNumber?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(studentNumber) || string.IsNullOrWhiteSpace(request.Password))
        {
            throw new UnauthorizedAccessException("Invalid student number or password.");
        }

        var student = await _studentRepository.GetStudentByStudentNumberAsync(studentNumber, cancellationToken);
        if (student == null || !student.HasPortalAccess())
        {
            throw new UnauthorizedAccessException("Invalid student number or password.");
        }

        if (!string.Equals(student.enrollment_status, "Active", StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException("Student account is not active.");
        }

        if (!PortalPasswordHelper.Verify(request.Password, student.portal_password_hash))
        {
            throw new UnauthorizedAccessException("Invalid student number or password.");
        }

        var expiresAt = DateTime.UtcNow.AddHours(GetExpirationHours());
        return new StudentPortalAuthResponse
        {
            Token = GenerateJwtToken(student, expiresAt),
            StudentId = student.id,
            StudentNumber = student.student_number,
            Name = FormatStudentName(student),
            ProgramCode = student.program_code,
            YearLevel = student.year_level,
            CurriculumCode = student.curriculum_code,
            Role = "Student",
            ExpiresAt = expiresAt
        };
    }

    public async Task<StudentResponse?> GetProfileAsync(long studentId, CancellationToken cancellationToken = default)
    {
        return await _studentService.GetStudentByIdAsync(studentId);
    }

    public async Task<StudentEnrollmentOverviewResponse?> GetEnrollmentsAsync(
        long studentId,
        CancellationToken cancellationToken = default)
    {
        return await _studentService.GetStudentEnrollmentOverviewAsync(studentId, cancellationToken);
    }

    public async Task<IReadOnlyList<StudentPortalPendingSubjectDto>> GetPendingSubjectsAsync(
        long studentId,
        CancellationToken cancellationToken = default)
    {
        var student = await _studentRepository.GetStudentByIdAsync(studentId);
        if (student == null || string.IsNullOrWhiteSpace(student.curriculum_code))
        {
            return Array.Empty<StudentPortalPendingSubjectDto>();
        }

        var overview = await _studentService.GetStudentEnrollmentOverviewAsync(studentId, cancellationToken);
        var passedCodes = BuildPassedCourseCodes(overview?.Enrollments ?? Array.Empty<StudentClassEnrollmentRowDto>());

        var coursesPage = await _courseRepository.GetAllCourses(new CourseRequest
        {
            CurriculumCode = student.curriculum_code.Trim(),
            PageIndex = 1,
            PageSize = 2000,
            SortDirection = "asc",
            SortKey = "course_code"
        });

        var pending = new List<StudentPortalPendingSubjectDto>();
        foreach (var course in coursesPage.Result)
        {
            if (course.is_elective_option)
            {
                continue;
            }

            var codeKey = NormalizeCode(course.course_code);
            if (string.IsNullOrEmpty(codeKey) || passedCodes.Contains(codeKey))
            {
                continue;
            }

            pending.Add(new StudentPortalPendingSubjectDto
            {
                CourseCode = course.course_code,
                SubjectDescription = course.course_title,
                Prerequisite = string.IsNullOrWhiteSpace(course.prerequisites) ? "None" : course.prerequisites.Trim(),
                Units = course.course_total_units,
                Component = string.IsNullOrWhiteSpace(course.course_component) ? "Lecture" : course.course_component.Trim(),
                YearTerm = BuildYearTerm(course.course_yearlevel, course.course_semester)
            });
        }

        return pending
            .OrderBy(p => p.YearTerm, StringComparer.OrdinalIgnoreCase)
            .ThenBy(p => p.CourseCode, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public async Task<StudentPortalDashboardResponse?> GetDashboardAsync(
        long studentId,
        CancellationToken cancellationToken = default)
    {
        var student = await _studentRepository.GetStudentByIdAsync(studentId);
        if (student == null)
        {
            return null;
        }

        var overview = await _studentService.GetStudentEnrollmentOverviewAsync(studentId, cancellationToken);
        var enrollments = overview?.Enrollments ?? Array.Empty<StudentClassEnrollmentRowDto>();
        var currentTerm = StudentYearLevelHelper.NormalizeYearTerm(student.year_level);
        var pending = await GetPendingSubjectsAsync(studentId, cancellationToken);
        var hasPendingReset = await _passwordResetRepository.HasPendingForStudentAsync(studentId, cancellationToken);

        return new StudentPortalDashboardResponse
        {
            StudentName = FormatStudentName(student),
            ProgramYearLevel = $"{student.program_code} - {currentTerm}",
            CurriculumCode = student.curriculum_code,
            EnrollmentStatus = student.enrollment_status,
            CurrentTermSubjects = enrollments.Count(e =>
                string.Equals(
                    StudentYearLevelHelper.NormalizeYearTerm(e.YearLevel),
                    currentTerm,
                    StringComparison.OrdinalIgnoreCase)),
            CompletedSubjects = enrollments.Count(e => IsPassedGrade(e.OfficialGrade)),
            PendingSubjects = pending.Count,
            CumulativeGpa = overview?.Summary.CumulativeGpa,
            TotalUnitsCompleted = overview?.Summary.TotalUnitsCompleted ?? 0,
            HasPendingPasswordReset = hasPendingReset
        };
    }

    public async Task ChangePasswordAsync(
        long studentId,
        StudentPortalChangePasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.NewPassword) || request.NewPassword.Length < 6)
        {
            throw new InvalidOperationException("New password must be at least 6 characters.");
        }

        var student = await _studentRepository.GetStudentByIdAsync(studentId);
        if (student == null || !student.HasPortalAccess())
        {
            throw new KeyNotFoundException("Student not found.");
        }

        if (!PortalPasswordHelper.Verify(request.CurrentPassword, student.portal_password_hash))
        {
            throw new UnauthorizedAccessException("Current password is incorrect.");
        }

        student.SetPortalPasswordHash(PortalPasswordHelper.Hash(request.NewPassword));
        await _studentRepository.UpdateStudentAsync(student);
    }

    public async Task RequestPasswordResetAsync(
        StudentPortalPasswordResetCreateRequest request,
        CancellationToken cancellationToken = default)
    {
        var studentNumber = request.StudentNumber?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(studentNumber))
        {
            throw new InvalidOperationException("Student number is required.");
        }

        var student = await _studentRepository.GetStudentByStudentNumberAsync(studentNumber, cancellationToken);
        if (student == null)
        {
            throw new KeyNotFoundException("Student number was not found.");
        }

        if (await _passwordResetRepository.HasPendingForStudentAsync(student.id, cancellationToken))
        {
            throw new InvalidOperationException("A password reset request is already pending for this student.");
        }

        var row = StudentPortalPasswordResetRequest.Create(student.id, student.student_number, request.Reason);
        await _passwordResetRepository.CreateAsync(row, cancellationToken);
    }

    public async Task<IReadOnlyList<StudentPortalPasswordResetRequestResponse>> ListPasswordResetRequestsAsync(
        string? status,
        CancellationToken cancellationToken = default)
    {
        var rows = await _passwordResetRepository.GetAllAsync(status, cancellationToken);
        var result = new List<StudentPortalPasswordResetRequestResponse>();

        foreach (var row in rows)
        {
            var student = await _studentRepository.GetStudentByIdAsync(row.student_id);
            result.Add(new StudentPortalPasswordResetRequestResponse
            {
                Id = row.id,
                StudentId = row.student_id,
                StudentNumber = row.student_number,
                StudentName = student == null ? row.student_number : FormatStudentName(student),
                Reason = row.reason,
                Status = row.status,
                RegistrarNotes = row.registrar_notes,
                ResolvedBy = row.resolved_by,
                RequestedAt = row.requested_at,
                ResolvedAt = row.resolved_at
            });
        }

        return result;
    }

    public async Task ResolvePasswordResetRequestAsync(
        long requestId,
        StudentPortalPasswordResetResolveRequest request,
        string? resolvedBy,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.NewPortalPassword) || request.NewPortalPassword.Length < 6)
        {
            throw new InvalidOperationException("New portal password must be at least 6 characters.");
        }

        var row = await _passwordResetRepository.GetByIdAsync(requestId, cancellationToken);
        if (row == null)
        {
            throw new KeyNotFoundException("Password reset request not found.");
        }

        if (!string.Equals(row.status, "Pending", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("This request has already been processed.");
        }

        var student = await _studentRepository.GetStudentByIdAsync(row.student_id);
        if (student == null)
        {
            throw new KeyNotFoundException("Student not found.");
        }

        student.SetPortalPasswordHash(PortalPasswordHelper.Hash(request.NewPortalPassword));
        await _studentRepository.UpdateStudentAsync(student);

        row.Resolve(request.RegistrarNotes, resolvedBy);
        await _passwordResetRepository.UpdateAsync(row, cancellationToken);
    }

    public async Task RejectPasswordResetRequestAsync(
        long requestId,
        StudentPortalPasswordResetRejectRequest request,
        string? resolvedBy,
        CancellationToken cancellationToken = default)
    {
        var row = await _passwordResetRepository.GetByIdAsync(requestId, cancellationToken);
        if (row == null)
        {
            throw new KeyNotFoundException("Password reset request not found.");
        }

        if (!string.Equals(row.status, "Pending", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("This request has already been processed.");
        }

        row.Reject(request.RegistrarNotes, resolvedBy);
        await _passwordResetRepository.UpdateAsync(row, cancellationToken);
    }

    private string GenerateJwtToken(Student student, DateTime expiresAt)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured.");
        var issuer = jwtSettings["Issuer"] ?? "EvuEase";
        var audience = jwtSettings["Audience"] ?? "EvuEase";

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("StudentId", student.id.ToString()),
            new Claim("StudentNumber", student.student_number),
            new Claim("UserName", FormatStudentName(student)),
            new Claim("Role", "Student"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private int GetExpirationHours()
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        return int.TryParse(jwtSettings["ExpirationHours"], out var hours) ? hours : 24;
    }

    private static string FormatStudentName(Student student)
    {
        var given = string.Join(' ', new[] { student.first_name, student.middle_name }.Where(s => !string.IsNullOrWhiteSpace(s)));
        return $"{student.last_name}, {given}".Trim().TrimEnd(',');
    }

    private static HashSet<string> BuildPassedCourseCodes(IReadOnlyList<StudentClassEnrollmentRowDto> enrollments)
    {
        var passed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var row in enrollments)
        {
            if (IsPassedGrade(row.OfficialGrade))
            {
                var key = NormalizeCode(row.CourseCode);
                if (!string.IsNullOrEmpty(key))
                {
                    passed.Add(key);
                }
            }
        }

        return passed;
    }

    private static bool IsPassedGrade(string? officialGrade)
    {
        if (string.IsNullOrWhiteSpace(officialGrade))
        {
            return false;
        }

        if (!decimal.TryParse(officialGrade.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var grade))
        {
            return false;
        }

        return grade > 0 && grade <= 3.0m;
    }

    private static string NormalizeCode(string? code) =>
        code?.Trim().ToLowerInvariant().Replace(" ", string.Empty, StringComparison.Ordinal) ?? string.Empty;

    private static string BuildYearTerm(string? yearLevel, string? semester)
    {
        var yearDigit = yearLevel?.Trim().FirstOrDefault(c => char.IsDigit(c)) ?? '1';
        var semDigit = semester != null && (semester.Contains("2", StringComparison.Ordinal) || semester.Contains("second", StringComparison.OrdinalIgnoreCase))
            ? '2'
            : '1';
        return $"{yearDigit}Y{semDigit}";
    }
}

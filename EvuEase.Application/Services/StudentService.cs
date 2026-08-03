using AutoMapper;
using EvuEase.Application.Common;
using EvuEase.Application.DTOs.Student;
using EvuEase.Application.DTOs.Course;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Application.Interfaces.Services;
using EvuEase.Application.EnrollmentAnalytics;
using EvuEase.Domain.Entities;
using System.Net.Mail;

namespace EvuEase.Application.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly IFacultyClassEnrollmentRepository _enrollmentRepository;
    private readonly ICurriculaRepository _curriculaRepository;
    private readonly IProgramRepository _programRepository;
    private readonly IStudentCurriculumHistoryRepository _curriculumHistoryRepository;
    private readonly IStudentCurriculumAssignmentService _curriculumAssignmentService;
    private readonly ICourseRepository _courseRepository;
    private readonly IMapper _mapper;

    public StudentService(
        IStudentRepository studentRepository,
        IFacultyClassEnrollmentRepository enrollmentRepository,
        ICurriculaRepository curriculaRepository,
        IProgramRepository programRepository,
        IStudentCurriculumHistoryRepository curriculumHistoryRepository,
        IStudentCurriculumAssignmentService curriculumAssignmentService,
        ICourseRepository courseRepository,
        IMapper mapper)
    {
        _studentRepository = studentRepository;
        _enrollmentRepository = enrollmentRepository;
        _curriculaRepository = curriculaRepository;
        _programRepository = programRepository;
        _curriculumHistoryRepository = curriculumHistoryRepository;
        _curriculumAssignmentService = curriculumAssignmentService;
        _courseRepository = courseRepository;
        _mapper = mapper;
    }

    public async Task<PagedResults<StudentResponse>> GetAllStudents(StudentRequest request)
    {
        var paged = await _studentRepository.GetAllStudents(request);
        var mapped = paged.MapToDto<Student, StudentResponse>(_mapper);
        await ApplyGraduationCandidateFlagsAsync(paged.Result, mapped.Result);
        return mapped;
    }

    public async Task<StudentResponse?> GetStudentByIdAsync(long id)
    {
        var student = await _studentRepository.GetStudentByIdAsync(id);
        return student == null ? null : _mapper.Map<StudentResponse>(student);
    }

    public async Task<StudentResponse> CreateStudentAsync(CreateStudentRequest request)
    {
        request.Address = NormalizeOptional(request.Address);
        request.ContactNumber = NormalizeOptional(request.ContactNumber);
        request.Email = NormalizeOptional(request.Email);
        request.Gender = NormalizeOptional(request.Gender);

        ValidateContactNumber(request.ContactNumber);
        ValidateEmail(request.Email);
        ValidateGender(request.Gender);
        ValidateBirthdate(request.Birthdate);

        if (await _studentRepository.StudentNumberExistsAsync(request.StudentNumber))
        {
            throw new InvalidOperationException($"Student number '{request.StudentNumber}' already exists.");
        }

        request.YearLevel = StudentYearLevelHelper.NormalizeYearTerm(request.YearLevel);

        var student = Student.Create(
            request.StudentNumber,
            request.FirstName,
            request.LastName,
            request.MiddleName,
            request.ProgramCode,
            request.ProgramTitle,
            request.YearLevel,
            request.StudentType,
            request.EnrollmentStatus,
            request.Address,
            request.ContactNumber,
            request.Email,
            request.Gender,
            request.Birthdate
        );

        var result = await _studentRepository.CreateStudentAsync(student);

        await ApplyPortalPasswordIfProvided(result, request.PortalPassword);

        await _curriculumAssignmentService.TryAssignDefaultCurriculumForFirstYearAsync(
            result,
            StudentCurriculumAssignmentService.DefaultFirstYearReason);

        return _mapper.Map<StudentResponse>(result);
    }

    public async Task<StudentResponse> UpdateStudentAsync(UpdateStudentRequest request)
    {
        request.Address = NormalizeOptional(request.Address);
        request.ContactNumber = NormalizeOptional(request.ContactNumber);
        request.Email = NormalizeOptional(request.Email);
        request.Gender = NormalizeOptional(request.Gender);

        ValidateContactNumber(request.ContactNumber);
        ValidateEmail(request.Email);
        ValidateGender(request.Gender);
        ValidateBirthdate(request.Birthdate);

        var student = await _studentRepository.GetStudentByIdAsync(request.Id);
        if (student == null)
        {
            throw new KeyNotFoundException($"Student with ID {request.Id} not found.");
        }

        if (await _studentRepository.StudentNumberExistsAsync(request.StudentNumber, request.Id))
        {
            throw new InvalidOperationException($"Student number '{request.StudentNumber}' already exists.");
        }

        request.YearLevel = StudentYearLevelHelper.NormalizeYearTerm(request.YearLevel);

        student.Update(
            request.StudentNumber,
            request.FirstName,
            request.LastName,
            request.MiddleName,
            request.ProgramCode,
            request.ProgramTitle,
            request.YearLevel,
            request.StudentType,
            request.EnrollmentStatus,
            request.Address,
            request.ContactNumber,
            request.Email,
            request.Gender,
            request.Birthdate
        );

        var result = await _studentRepository.UpdateStudentAsync(student);
        await ApplyPortalPasswordIfProvided(result, request.PortalPassword);
        return _mapper.Map<StudentResponse>(result);
    }

    private async Task ApplyPortalPasswordIfProvided(Student student, string? portalPassword)
    {
        if (portalPassword == null)
        {
            return;
        }

        var trimmed = portalPassword.Trim();
        if (trimmed.Length == 0)
        {
            return;
        }

        if (trimmed.Length < 6)
        {
            throw new InvalidOperationException("Portal password must be at least 6 characters.");
        }

        student.SetPortalPasswordHash(PortalPasswordHelper.Hash(trimmed));
        await _studentRepository.UpdateStudentAsync(student);
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static void ValidateContactNumber(string? contactNumber)
    {
        if (string.IsNullOrWhiteSpace(contactNumber))
        {
            return;
        }

        var digitCount = 0;
        foreach (var c in contactNumber)
        {
            if (char.IsDigit(c))
            {
                digitCount++;
            }
        }
        if (digitCount is < 7 or > 15)
        {
            throw new InvalidOperationException("Contact number must contain 7 to 15 digits.");
        }
    }

    private static void ValidateEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return;
        }

        try
        {
            _ = new MailAddress(email);
        }
        catch (FormatException)
        {
            throw new InvalidOperationException("Email address format is invalid.");
        }
    }

    private static void ValidateGender(string? gender)
    {
        if (string.IsNullOrWhiteSpace(gender))
        {
            return;
        }

        if (!string.Equals(gender, "Male", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(gender, "Female", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Gender must be Male or Female.");
        }
    }

    private static void ValidateBirthdate(DateOnly? birthdate)
    {
        if (birthdate == null)
        {
            return;
        }

        if (birthdate > DateOnly.FromDateTime(DateTime.Today))
        {
            throw new InvalidOperationException("Birthdate cannot be in the future.");
        }
    }

    public async Task DeleteStudentAsync(long id)
    {
        var student = await _studentRepository.GetStudentByIdAsync(id);
        if (student == null)
        {
            throw new KeyNotFoundException($"Student with ID {id} not found.");
        }

        await _studentRepository.DeleteStudentAsync(student);
    }

    public async Task<StudentEnrollmentOverviewResponse?> GetStudentEnrollmentOverviewAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var student = await _studentRepository.GetStudentByIdAsync(id);
        if (student == null)
        {
            return null;
        }

        var rows = await _enrollmentRepository.GetEnrollmentRowsForStudentAsync(id, cancellationToken);
        var list = rows.ToList();
        var summary = StudentEnrollmentAnalytics.ComputeSummary(list);
        return new StudentEnrollmentOverviewResponse
        {
            Enrollments = list,
            Summary = summary
        };
    }

    public async Task<StudentResponse?> MigrateStudentCurriculumAsync(
        long id,
        MigrateStudentCurriculumRequest request,
        string? migratedBy,
        CancellationToken cancellationToken = default)
    {
        var code = request.CurriculumCode?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(code))
        {
            throw new InvalidOperationException("Curriculum code is required.");
        }

        var student = await _studentRepository.GetStudentByIdAsync(id);
        if (student == null)
        {
            return null;
        }

        var curriculum = await _curriculaRepository.GetCurriculaByCodeAsync(code);
        if (curriculum == null)
        {
            throw new InvalidOperationException($"Curriculum '{code}' was not found.");
        }

        if (!CurriculumStatusHelper.IsActive(curriculum.curriculum_status))
        {
            throw new InvalidOperationException($"Curriculum '{code}' is inactive and cannot be assigned.");
        }

        var program = await _programRepository.GetProgramByCodeAsync(student.program_code);
        if (program == null || curriculum.program_id != program.program_id)
        {
            throw new InvalidOperationException("Curriculum must belong to the student's program.");
        }

        student.SetCurriculumCode(code);
        await _studentRepository.UpdateStudentAsync(student);

        var history = StudentCurriculumHistory.Create(
            id,
            code,
            request.EffectiveSchoolYear,
            request.Reason,
            request.Notes,
            migratedBy);

        await _curriculumHistoryRepository.CreateAsync(history, cancellationToken);

        return _mapper.Map<StudentResponse>(student);
    }

    public async Task<IReadOnlyList<StudentCurriculumHistoryResponse>?> GetStudentCurriculumHistoryAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var student = await _studentRepository.GetStudentByIdAsync(id);
        if (student == null)
        {
            return null;
        }

        var rows = await _curriculumHistoryRepository.GetByStudentIdAsync(id, cancellationToken);
        var currentCode = student.curriculum_code?.Trim() ?? string.Empty;

        return rows
            .Select(h =>
            {
                var dto = _mapper.Map<StudentCurriculumHistoryResponse>(h);
                dto.IsCurrent = !string.IsNullOrEmpty(currentCode)
                    && string.Equals(h.curriculum_code, currentCode, StringComparison.OrdinalIgnoreCase);
                return dto;
            })
            .ToList();
    }

    private async Task ApplyGraduationCandidateFlagsAsync(
        IReadOnlyList<Student> students,
        IList<StudentResponse> responses)
    {
        if (students.Count == 0 || responses.Count == 0)
        {
            return;
        }

        var studentIds = students.Select(s => s.id).ToList();
        var enrollmentsByStudent = await _enrollmentRepository.GetEnrollmentRowsForStudentsAsync(studentIds);

        var curriculumCodes = students
            .Select(s => s.curriculum_code?.Trim())
            .Where(code => !string.IsNullOrEmpty(code))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Cast<string>()
            .ToList();

        var requiredCoursesByCurriculum = new Dictionary<string, List<StudentGraduationEligibilityHelper.GraduationRequiredCourse>>(
            StringComparer.OrdinalIgnoreCase);

        foreach (var curriculumCode in curriculumCodes)
        {
            var coursesPage = await _courseRepository.GetAllCourses(new CourseRequest
            {
                CurriculumCode = curriculumCode,
                PageIndex = 1,
                PageSize = 2000,
                SortDirection = "asc",
                SortKey = "course_code"
            });

            requiredCoursesByCurriculum[curriculumCode] = coursesPage.Result
                .Select(c => new StudentGraduationEligibilityHelper.GraduationRequiredCourse(
                    c.course_code,
                    c.is_elective_option))
                .ToList();
        }

        for (var i = 0; i < students.Count; i++)
        {
            var student = students[i];
            var response = responses[i];
            var curriculumCode = student.curriculum_code?.Trim();

            if (string.IsNullOrEmpty(curriculumCode)
                || !requiredCoursesByCurriculum.TryGetValue(curriculumCode, out var requiredCourses))
            {
                response.IsCandidateForGraduation = false;
                continue;
            }

            enrollmentsByStudent.TryGetValue(student.id, out var enrollments);
            enrollments ??= Array.Empty<StudentClassEnrollmentRowDto>();

            response.IsCandidateForGraduation = StudentGraduationEligibilityHelper.IsCandidateForGraduation(
                requiredCourses,
                enrollments);
        }
    }
}

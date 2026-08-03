using EvuEase.Application.ClassRoster;
using EvuEase.Application.Common;
using EvuEase.Application.DTOs.CreditRequest;
using EvuEase.Application.Interfaces.Persistence;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Application.Interfaces.Services;
using EvuEase.Domain.Entities;

namespace EvuEase.Application.Services;

public class CreditRequestService : ICreditRequestService
{
    private const string CreditClassSection = "TC";
    private const string CreditRemarks = "Credited";

    private readonly ICreditRequestRepository _creditRequestRepository;
    private readonly ICreditRequestLineRepository _lineRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IProgramRepository _programRepository;
    private readonly ISyTermRepository _syTermRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IFacultyClassRepository _facultyClassRepository;
    private readonly IFacultyClassEnrollmentRepository _enrollmentRepository;
    private readonly ICreditRequestSignedDocumentStore _signedDocumentStore;
    private readonly IApplicationUnitOfWork _unitOfWork;

    public CreditRequestService(
        ICreditRequestRepository creditRequestRepository,
        ICreditRequestLineRepository lineRepository,
        IStudentRepository studentRepository,
        IProgramRepository programRepository,
        ISyTermRepository syTermRepository,
        ICourseRepository courseRepository,
        IFacultyClassRepository facultyClassRepository,
        IFacultyClassEnrollmentRepository enrollmentRepository,
        ICreditRequestSignedDocumentStore signedDocumentStore,
        IApplicationUnitOfWork unitOfWork)
    {
        _creditRequestRepository = creditRequestRepository;
        _lineRepository = lineRepository;
        _studentRepository = studentRepository;
        _programRepository = programRepository;
        _syTermRepository = syTermRepository;
        _courseRepository = courseRepository;
        _facultyClassRepository = facultyClassRepository;
        _enrollmentRepository = enrollmentRepository;
        _signedDocumentStore = signedDocumentStore;
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResults<CreditRequestResponse>> GetAllCreditRequestsAsync(
        CreditRequestRequest request,
        CancellationToken cancellationToken = default)
    {
        var rows = await _creditRequestRepository.GetAllAsync(request, cancellationToken);
        var total = await _creditRequestRepository.CountAllAsync(request, cancellationToken);
        var responses = new List<CreditRequestResponse>();

        foreach (var row in rows)
        {
            responses.Add(await MapToResponseAsync(row, includeLines: false, cancellationToken));
        }

        return new PagedResults<CreditRequestResponse>(
            request.PageIndex,
            request.PageSize,
            total,
            total,
            responses);
    }

    public async Task<CreditRequestResponse?> GetCreditRequestByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _creditRequestRepository.GetByIdAsync(id, cancellationToken);
        if (entity == null)
        {
            return null;
        }

        return await MapToResponseAsync(entity, includeLines: true, cancellationToken);
    }

    public async Task<CreditRequestResponse> CreateCreditRequestAsync(
        CreateCreditRequestRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var studentNumber = request.StudentNumber?.Trim() ?? string.Empty;
        var firstName = request.FirstName?.Trim() ?? string.Empty;
        var lastName = request.LastName?.Trim() ?? string.Empty;
        var middleName = string.IsNullOrWhiteSpace(request.MiddleName) ? null : request.MiddleName.Trim();

        if (string.IsNullOrEmpty(studentNumber) || string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
        {
            throw new ArgumentException("Student number, first name, and last name are required.");
        }

        if (request.ProgramId <= 0)
        {
            throw new ArgumentException("Program is required.");
        }

        if (request.SyId <= 0)
        {
            throw new ArgumentException("School year and term are required.");
        }

        var program = await _programRepository.GetProgramByIdAsync(request.ProgramId);
        if (program == null)
        {
            throw new ArgumentException("Selected program was not found.");
        }

        var syTerm = await _syTermRepository.GetSyTermByIdAsync(request.SyId);
        if (syTerm == null)
        {
            throw new ArgumentException("Selected school year and term was not found.");
        }

        var normalizedLines = NormalizeLines(request.Lines);
        if (normalizedLines.Count == 0)
        {
            throw new ArgumentException("At least one course credit line is required.");
        }

        foreach (var line in normalizedLines)
        {
            if (string.IsNullOrWhiteSpace(line.EquivalentCourseCode))
            {
                continue;
            }

            var course = await _courseRepository.GetCourseByCodeAsync(line.EquivalentCourseCode);
            if (course == null)
            {
                throw new ArgumentException($"Equivalent STI course \"{line.EquivalentCourseCode}\" was not found.");
            }
        }

        var existingStudent = await _studentRepository.GetStudentByStudentNumberAsync(studentNumber, cancellationToken);

        var entity = CreditRequest.Create(
            studentNumber,
            firstName,
            middleName,
            lastName,
            existingStudent?.id,
            request.ProgramId,
            request.SyId);

        var created = await _creditRequestRepository.CreateAsync(entity, cancellationToken);
        created.SetCreditRequestNo($"CR-{created.id:D6}");
        await _creditRequestRepository.UpdateCreditRequestAsync(created, cancellationToken);
        await _lineRepository.ReplaceForCreditRequestAsync(created.id, normalizedLines, cancellationToken);

        return (await GetCreditRequestByIdAsync(created.id, cancellationToken))!;
    }

    public async Task<CreditRequestResponse?> UpdateCreditRequestStatusAsync(
        long id,
        UpdateCreditRequestStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var status = request.RequestStatus?.Trim() ?? string.Empty;
        if (!IsValidRequestStatus(status))
        {
            throw new ArgumentException("Request status must be Pending, Approved, or Rejected.");
        }

        var entity = await _creditRequestRepository.GetByIdAsync(id, cancellationToken);
        if (entity == null)
        {
            return null;
        }

        var previousStatus = entity.request_status?.Trim() ?? "Pending";
        if (previousStatus.Equals(status, StringComparison.OrdinalIgnoreCase))
        {
            return await GetCreditRequestByIdAsync(id, cancellationToken);
        }

        if (status.Equals("Approved", StringComparison.OrdinalIgnoreCase))
        {
            await using var tx = await _unitOfWork.BeginTransactionAsync(cancellationToken);
            await ApplyApprovedCreditsAsync(entity, cancellationToken);
            entity.SetRequestStatus(status);
            await _creditRequestRepository.UpdateCreditRequestAsync(entity, cancellationToken);
            await tx.CommitAsync(cancellationToken);
            return await GetCreditRequestByIdAsync(id, cancellationToken);
        }

        entity.SetRequestStatus(status);
        await _creditRequestRepository.UpdateCreditRequestAsync(entity, cancellationToken);
        return await GetCreditRequestByIdAsync(id, cancellationToken);
    }

    public async Task<CreditRequestResponse?> UploadSignedPdfAsync(
        long id,
        Stream content,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        var entity = await _creditRequestRepository.GetByIdAsync(id, cancellationToken);
        if (entity == null)
        {
            return null;
        }

        if (content == null || !content.CanRead)
        {
            throw new ArgumentException("A PDF file is required.");
        }

        var normalizedName = fileName?.Trim() ?? string.Empty;
        if (!normalizedName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Only PDF files are accepted.");
        }

        if (!string.IsNullOrWhiteSpace(entity.signed_pdf_storage_key))
        {
            await _signedDocumentStore.DeleteSignedPdfAsync(entity.signed_pdf_storage_key, cancellationToken);
        }

        var saved = await _signedDocumentStore.SaveSignedPdfAsync(id, content, normalizedName, cancellationToken);
        entity.SetSignedPdf(saved.FileName, saved.StorageKey);
        await _creditRequestRepository.UpdateCreditRequestAsync(entity, cancellationToken);

        return await GetCreditRequestByIdAsync(id, cancellationToken);
    }

    public async Task<(Stream Stream, string FileName)?> GetSignedPdfAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _creditRequestRepository.GetByIdAsync(id, cancellationToken);
        if (entity == null || string.IsNullOrWhiteSpace(entity.signed_pdf_storage_key))
        {
            return null;
        }

        var stream = await _signedDocumentStore.OpenSignedPdfAsync(entity.signed_pdf_storage_key, cancellationToken);
        if (stream == null)
        {
            return null;
        }

        var fileName = string.IsNullOrWhiteSpace(entity.signed_pdf_file_name)
            ? "signed-credit-request.pdf"
            : entity.signed_pdf_file_name;

        return (stream, fileName);
    }

    public async Task<CreditRequestResponse?> RemoveSignedPdfAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _creditRequestRepository.GetByIdAsync(id, cancellationToken);
        if (entity == null)
        {
            return null;
        }

        await _signedDocumentStore.DeleteSignedPdfAsync(entity.signed_pdf_storage_key, cancellationToken);
        entity.SetSignedPdf(null, null);
        await _creditRequestRepository.UpdateCreditRequestAsync(entity, cancellationToken);

        return await GetCreditRequestByIdAsync(id, cancellationToken);
    }

    private async Task ApplyApprovedCreditsAsync(CreditRequest entity, CancellationToken cancellationToken)
    {
        var student = await ResolveStudentAsync(entity, cancellationToken);
        if (student == null)
        {
            throw new ArgumentException(
                "The student must be registered in the system before credit subjects can be applied.");
        }

        if (entity.student_id != student.id)
        {
            entity.SetStudentId(student.id);
        }

        var program = await _programRepository.GetProgramByIdAsync(entity.program_id);
        if (program == null)
        {
            throw new ArgumentException("Program for this credit request was not found.");
        }

        var syTerm = await _syTermRepository.GetSyTermByIdAsync(entity.sy_id);
        if (syTerm == null)
        {
            throw new ArgumentException("School year and term for this credit request was not found.");
        }

        var academicTerm = ClassListPdfClassHeaderParser.NormalizeAcademicPeriodLabel(
            FormatAcademicTermLabel(syTerm.sy_year, syTerm.sy_semester));
        var termKeys = ClassListPdfClassHeaderParser.GetAcademicPeriodMatchKeys(academicTerm);
        if (termKeys.Count == 0)
        {
            throw new ArgumentException("Academic term for this credit request is invalid.");
        }

        var lines = await _lineRepository.GetByCreditRequestIdAsync(entity.id, cancellationToken);
        var creditedCount = 0;

        foreach (var line in lines)
        {
            var equivalentCode = line.equivalent_course_code?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(equivalentCode))
            {
                continue;
            }

            if (await _enrollmentRepository.StudentHasCourseEnrollmentAsync(student.id, equivalentCode, cancellationToken))
            {
                continue;
            }

            var course = await _courseRepository.GetCourseByCodeAsync(equivalentCode);
            if (course == null)
            {
                throw new ArgumentException($"Equivalent STI course \"{equivalentCode}\" was not found.");
            }

            var classNumber = BuildCreditClassNumber(equivalentCode);
            var facultyClass = await _facultyClassRepository.FindByCourseClassNumberAndTermAsync(
                equivalentCode,
                classNumber,
                termKeys,
                cancellationToken);

            if (facultyClass == null)
            {
                var component = string.IsNullOrWhiteSpace(course.course_component) ? "LEC" : course.course_component.Trim();
                facultyClass = await _facultyClassRepository.CreateAsync(
                    FacultyClass.Create(
                        equivalentCode,
                        classNumber,
                        CreditClassSection,
                        course.course_title,
                        component,
                        academicTerm,
                        0,
                        program.program_code,
                        course.course_yearlevel),
                    cancellationToken);
            }

            var enrollment = await _enrollmentRepository.AddEnrollmentAsync(facultyClass.id, student.id, cancellationToken);
            if (enrollment == null)
            {
                continue;
            }

            var grade = string.IsNullOrWhiteSpace(line.grade) ? null : line.grade.Trim();
            await _enrollmentRepository.UpdateOfficialGradeAsync(
                enrollment.Id,
                grade,
                CreditRemarks,
                cancellationToken);

            var count = await _enrollmentRepository.CountEnrollmentsForClassAsync(facultyClass.id, cancellationToken);
            await _facultyClassRepository.UpdateEnrolledCountAsync(facultyClass.id, count, cancellationToken);
            creditedCount++;
        }

        if (creditedCount == 0 && lines.Any(l => !string.IsNullOrWhiteSpace(l.equivalent_course_code)))
        {
            var hasEquivalent = lines.Any(l => !string.IsNullOrWhiteSpace(l.equivalent_course_code));
            if (hasEquivalent)
            {
                var allAlreadyCredited = true;
                foreach (var line in lines)
                {
                    var code = line.equivalent_course_code?.Trim() ?? string.Empty;
                    if (string.IsNullOrEmpty(code))
                    {
                        continue;
                    }

                    if (!await _enrollmentRepository.StudentHasCourseEnrollmentAsync(student.id, code, cancellationToken))
                    {
                        allAlreadyCredited = false;
                        break;
                    }
                }

                if (!allAlreadyCredited)
                {
                    throw new InvalidOperationException("No subjects could be credited for this request.");
                }
            }
        }
    }

    private async Task<Student?> ResolveStudentAsync(CreditRequest entity, CancellationToken cancellationToken)
    {
        if (entity.student_id.HasValue && entity.student_id.Value > 0)
        {
            var byId = await _studentRepository.GetStudentByIdAsync(entity.student_id.Value);
            if (byId != null)
            {
                return byId;
            }
        }

        return await _studentRepository.GetStudentByStudentNumberAsync(entity.student_number, cancellationToken);
    }

    private static string BuildCreditClassNumber(string courseCode)
    {
        var code = courseCode.Trim().ToUpperInvariant();
        var candidate = $"CR-{code}";
        return candidate.Length <= 32 ? candidate : candidate[..32];
    }

    private static string FormatAcademicTermLabel(string syYear, string sySemester)
    {
        var year = syYear?.Trim() ?? string.Empty;
        var semester = sySemester?.Trim() ?? string.Empty;
        if (!string.IsNullOrEmpty(year) && !string.IsNullOrEmpty(semester))
        {
            return $"{year} / {semester}";
        }

        return year.Length > 0 ? year : semester;
    }

    private static bool IsValidRequestStatus(string status)
    {
        return status.Equals("Pending", StringComparison.OrdinalIgnoreCase)
            || status.Equals("Approved", StringComparison.OrdinalIgnoreCase)
            || status.Equals("Rejected", StringComparison.OrdinalIgnoreCase);
    }

    private static List<CreateCreditRequestLineRequest> NormalizeLines(IEnumerable<CreateCreditRequestLineRequest>? lines)
    {
        var result = new List<CreateCreditRequestLineRequest>();
        if (lines == null)
        {
            return result;
        }

        foreach (var line in lines)
        {
            var appliedCode = line.AppliedCourseCode?.Trim() ?? string.Empty;
            var appliedTitle = line.AppliedCourseTitle?.Trim() ?? string.Empty;
            var equivalentCode = line.EquivalentCourseCode?.Trim() ?? string.Empty;
            var hasApplied = !string.IsNullOrEmpty(appliedCode) || !string.IsNullOrEmpty(appliedTitle);
            var hasEquivalent = !string.IsNullOrEmpty(equivalentCode);
            var hasUnits = line.AppliedLecUnits > 0 || line.AppliedLabUnits > 0;

            if (!hasApplied && !hasEquivalent && !hasUnits)
            {
                continue;
            }

            result.Add(new CreateCreditRequestLineRequest
            {
                AppliedCourseCode = appliedCode,
                AppliedCourseTitle = appliedTitle,
                AppliedLecUnits = line.AppliedLecUnits,
                AppliedLabUnits = line.AppliedLabUnits,
                Grade = line.Grade?.Trim(),
                EquivalentCourseCode = equivalentCode
            });
        }

        return result;
    }

    private async Task<CreditRequestResponse> MapToResponseAsync(
        CreditRequest entity,
        bool includeLines,
        CancellationToken cancellationToken)
    {
        var program = await _programRepository.GetProgramByIdAsync(entity.program_id);
        var syTerm = await _syTermRepository.GetSyTermByIdAsync(entity.sy_id);

        var response = new CreditRequestResponse
        {
            Id = entity.id,
            CreditRequestNo = entity.credit_request_no,
            StudentId = entity.student_id,
            StudentNumber = entity.student_number,
            FirstName = entity.first_name,
            MiddleName = entity.middle_name,
            LastName = entity.last_name,
            StudentName = FormatStudentName(entity.last_name, entity.first_name, entity.middle_name),
            ProgramId = entity.program_id,
            ProgramCode = program?.program_code ?? string.Empty,
            ProgramTitle = program?.program_title ?? string.Empty,
            SyId = entity.sy_id,
            SyCode = syTerm?.sy_code ?? string.Empty,
            SyYear = syTerm?.sy_year ?? string.Empty,
            SySemester = syTerm?.sy_semester ?? string.Empty,
            RequestStatus = entity.request_status,
            SignedPdfFileName = entity.signed_pdf_file_name,
            HasSignedPdf = !string.IsNullOrWhiteSpace(entity.signed_pdf_storage_key)
        };

        if (!includeLines)
        {
            return response;
        }

        var lines = await _lineRepository.GetByCreditRequestIdAsync(entity.id, cancellationToken);
        foreach (var line in lines)
        {
            Course? equivalentCourse = null;
            if (!string.IsNullOrWhiteSpace(line.equivalent_course_code))
            {
                equivalentCourse = await _courseRepository.GetCourseByCodeAsync(line.equivalent_course_code);
            }

            response.Lines.Add(new CreditRequestLineResponse
            {
                Id = line.id,
                SortOrder = line.sort_order,
                AppliedCourseCode = line.applied_course_code,
                AppliedCourseTitle = line.applied_course_title,
                AppliedLecUnits = line.applied_lec_units,
                AppliedLabUnits = line.applied_lab_units,
                Grade = line.grade,
                EquivalentCourseCode = line.equivalent_course_code,
                EquivalentCourseTitle = equivalentCourse?.course_title,
                EquivalentLecUnits = equivalentCourse?.course_lec_units,
                EquivalentLabUnits = equivalentCourse?.course_lab_units,
                EquivalentTotalUnits = equivalentCourse?.course_total_units
            });
        }

        return response;
    }

    private static string FormatStudentName(string lastName, string firstName, string? middleName)
    {
        var middle = string.IsNullOrWhiteSpace(middleName) ? string.Empty : $" {middleName.Trim()}";
        return $"{lastName}, {firstName}{middle}";
    }
}

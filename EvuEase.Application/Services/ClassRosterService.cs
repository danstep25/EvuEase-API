using System.Globalization;
using System.Text.RegularExpressions;
using EvuEase.Application.ClassRoster;
using EvuEase.Application.DTOs.ClassRoster;
using EvuEase.Application.DTOs.GradeRoster;
using EvuEase.Application.Interfaces.Persistence;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Application.Interfaces.Services;
using EvuEase.Domain.Entities;

namespace EvuEase.Application.Services;

public class ClassRosterService : IClassRosterService
{
    private readonly IFacultyClassRepository _repository;
    private readonly IFacultyClassEnrollmentRepository _enrollmentRepository;
    private readonly IGradeScaleRowRepository _gradeScaleRowRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IProgramRepository _programRepository;
    private readonly IApplicationUnitOfWork _unitOfWork;
    private readonly ICourseService _courseService;

    public ClassRosterService(
        IFacultyClassRepository repository,
        IFacultyClassEnrollmentRepository enrollmentRepository,
        IGradeScaleRowRepository gradeScaleRowRepository,
        IStudentRepository studentRepository,
        IProgramRepository programRepository,
        IApplicationUnitOfWork unitOfWork,
        ICourseService courseService)
    {
        _repository = repository;
        _enrollmentRepository = enrollmentRepository;
        _gradeScaleRowRepository = gradeScaleRowRepository;
        _studentRepository = studentRepository;
        _programRepository = programRepository;
        _unitOfWork = unitOfWork;
        _courseService = courseService;
    }

    public async Task<IReadOnlyList<ClassRosterResponse>> GetClassesAsync(string? search, CancellationToken cancellationToken = default)
    {
        var rows = await _repository.GetAllAsync(search, cancellationToken);
        return rows.Select(MapToResponse).ToList();
    }

    public async Task<IReadOnlyList<GradeRosterClassLookupResponse>> GetGradeRosterClassLookupAsync(
        string academicTerm,
        string? search,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(academicTerm))
        {
            throw new ArgumentException("Academic term is required.", nameof(academicTerm));
        }

        var keys = ClassListPdfClassHeaderParser.GetAcademicPeriodMatchKeys(academicTerm);
        if (keys.Count == 0)
        {
            return Array.Empty<GradeRosterClassLookupResponse>();
        }

        var rows = await _repository.GetForGradeRosterLookupAsync(keys, search, cancellationToken);
        return MapToGradeRosterClassLookup(rows);
    }

    public async Task<ClassRosterResponse> CreateClassAsync(CreateClassRosterRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var courseCode = request.CourseCode?.Trim() ?? string.Empty;
        var classNumber = request.ClassNumber?.Trim() ?? string.Empty;
        var section = request.Section?.Trim() ?? string.Empty;
        var courseTitle = request.CourseTitle?.Trim() ?? string.Empty;
        var component = request.Component?.Trim() ?? string.Empty;
        var academicTerm = ClassListPdfClassHeaderParser.NormalizeAcademicPeriodLabel(
            request.AcademicTerm?.Trim() ?? string.Empty);
        var programCode = request.ProgramCode?.Trim() ?? string.Empty;
        var yearLevel = request.YearLevel?.Trim() ?? string.Empty;

        if (string.IsNullOrEmpty(courseCode) || string.IsNullOrEmpty(classNumber) ||
            string.IsNullOrEmpty(section) || string.IsNullOrEmpty(courseTitle) ||
            string.IsNullOrEmpty(component) || string.IsNullOrEmpty(academicTerm) ||
            string.IsNullOrEmpty(programCode) || string.IsNullOrEmpty(yearLevel))
        {
            throw new ArgumentException("All class fields are required.");
        }

        if (request.Enrolled < 0)
        {
            throw new ArgumentException("Enrolled count cannot be negative.");
        }

        var entity = FacultyClass.Create(
            courseCode,
            classNumber,
            section,
            courseTitle,
            component,
            academicTerm,
            request.Enrolled,
            programCode,
            yearLevel);

        var created = await _repository.CreateAsync(entity, cancellationToken);
        return MapToResponse(created);
    }

    public async Task DeleteClassAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity == null)
        {
            throw new KeyNotFoundException($"Class with ID {id} was not found.");
        }

        await using var tx = await _unitOfWork.BeginTransactionAsync(cancellationToken);
        await _enrollmentRepository.ReplaceAllForClassAsync(id, Array.Empty<FacultyClassEnrollment>(), cancellationToken);
        await _repository.SoftDeleteFacultyClassAsync(entity, cancellationToken);
        await tx.CommitAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ClassRosterStudentResponse>> GetStudentsForClassAsync(long facultyClassId, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(facultyClassId, cancellationToken);
        if (entity == null)
        {
            throw new KeyNotFoundException($"Class with ID {facultyClassId} was not found.");
        }

        return await _enrollmentRepository.GetStudentRowsForClassAsync(facultyClassId, cancellationToken);
    }

    public async Task<ClassRosterStudentResponse> AddStudentToClassAsync(
        long facultyClassId,
        long studentId,
        CancellationToken cancellationToken = default)
    {
        if (studentId <= 0)
        {
            throw new ArgumentException("A valid student ID is required.", nameof(studentId));
        }

        var cls = await _repository.GetByIdAsync(facultyClassId, cancellationToken);
        if (cls == null)
        {
            throw new KeyNotFoundException($"Class with ID {facultyClassId} was not found.");
        }

        var student = await _studentRepository.GetStudentByIdAsync(studentId);
        if (student == null)
        {
            throw new KeyNotFoundException($"Student with ID {studentId} was not found.");
        }

        await using var tx = await _unitOfWork.BeginTransactionAsync(cancellationToken);
        var row = await _enrollmentRepository.AddEnrollmentAsync(facultyClassId, studentId, cancellationToken);
        if (row == null)
        {
            throw new InvalidOperationException("This student is already enrolled in this class.");
        }

        var count = await _enrollmentRepository.CountEnrollmentsForClassAsync(facultyClassId, cancellationToken);
        await _repository.UpdateEnrolledCountAsync(facultyClassId, count, cancellationToken);
        await tx.CommitAsync(cancellationToken);

        return row;
    }

    public async Task RemoveStudentFromClassAsync(
        long facultyClassId,
        long enrollmentId,
        CancellationToken cancellationToken = default)
    {
        var cls = await _repository.GetByIdAsync(facultyClassId, cancellationToken);
        if (cls == null)
        {
            throw new KeyNotFoundException($"Class with ID {facultyClassId} was not found.");
        }

        await using var tx = await _unitOfWork.BeginTransactionAsync(cancellationToken);
        var removed = await _enrollmentRepository.RemoveEnrollmentAsync(facultyClassId, enrollmentId, cancellationToken);
        if (!removed)
        {
            throw new KeyNotFoundException("Enrollment was not found for this class.");
        }

        var count = await _enrollmentRepository.CountEnrollmentsForClassAsync(facultyClassId, cancellationToken);
        await _repository.UpdateEnrolledCountAsync(facultyClassId, count, cancellationToken);
        await tx.CommitAsync(cancellationToken);
    }

    public async Task<ClassRosterStudentResponse> UpdateEnrollmentGradeAsync(
        long enrollmentId,
        UpdateEnrollmentGradeRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.RawMark.HasValue)
        {
            var key = request.AcademicTermKey?.Trim();
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Academic term key is required when submitting a raw mark.");
            }

            var rows = await _gradeScaleRowRepository.GetByAcademicTermKeyAsync(key, cancellationToken);
            if (rows.Count == 0)
            {
                throw new ArgumentException(
                    "No grade scale is configured for this term. Add grade scale rows in Class Assignment.");
            }

            var resolved = GradeScaleMarkResolver.ResolveOfficialGrade(rows, request.RawMark.Value);
            if (resolved == null)
            {
                throw new ArgumentException(
                    "The score does not reach any mark threshold in the grade scale. Adjust the score or the scale in Class Assignment.");
            }

            var normalized = resolved.Value.ToString("0.##", CultureInfo.InvariantCulture);
            var updated = await _enrollmentRepository.UpdateOfficialGradeAsync(
                enrollmentId,
                normalized,
                storedRemarks: null,
                cancellationToken);
            if (updated == null)
            {
                throw new KeyNotFoundException($"Enrollment with ID {enrollmentId} was not found.");
            }

            return updated;
        }

        var raw = request.OfficialGrade?.Trim();
        var normalizedDirect = string.IsNullOrEmpty(raw) ? null : raw;
        var remarks = request.Remarks?.Trim();
        if (string.IsNullOrEmpty(remarks))
        {
            remarks = null;
        }

        var updatedRow = await _enrollmentRepository.UpdateOfficialGradeAsync(
            enrollmentId,
            normalizedDirect,
            remarks,
            cancellationToken);
        if (updatedRow == null)
        {
            throw new KeyNotFoundException($"Enrollment with ID {enrollmentId} was not found.");
        }

        return updatedRow;
    }

    public async Task<ClassRosterBatchUploadResponse> BatchUploadRosterPdfAsync(
        long facultyClassId,
        Stream pdfStream,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pdfStream);

        var entity = await _repository.GetByIdAsync(facultyClassId, cancellationToken);
        if (entity == null)
        {
            throw new KeyNotFoundException($"Class with ID {facultyClassId} was not found.");
        }

        IReadOnlyList<ClassListPdfParser.ParsedRow> parsed;
        try
        {
            parsed = ClassListPdfParser.Parse(pdfStream);
        }
        catch (Exception ex)
        {
            throw new ArgumentException("Could not read the PDF. Ensure it is a valid PDF file.", ex);
        }

        if (parsed.Count == 0)
        {
            throw new ArgumentException(
                "No student rows were found. Use the official class list PDF (columns: Student No, Name, Program, Level).");
        }

        var prepared = await PrepareEnrollmentsAsync(facultyClassId, parsed, cancellationToken);
        await ApplyRosterReplaceInTransactionAsync(facultyClassId, prepared.Enrollments, cancellationToken);

        return new ClassRosterBatchUploadResponse
        {
            ImportedCount = prepared.Enrollments.Count,
            AutoCreatedCount = prepared.AutoCreatedCount,
            NotFoundInRegistry = prepared.NotFoundInRegistry,
            Warnings = prepared.Warnings
        };
    }

    public async Task<ClassRosterPdfImportSummaryResponse> ImportClassRosterPdfAsync(
        Stream pdfStream,
        IReadOnlyCollection<string>? includedRowKeys = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pdfStream);

        IReadOnlyList<string> pageTexts;
        try
        {
            pageTexts = ClassListPdfParser.ExtractPageTexts(pdfStream);
        }
        catch (Exception ex)
        {
            throw new ArgumentException("Could not read the PDF. Ensure it is a valid PDF file.", ex);
        }

        if (pageTexts.Count == 0)
        {
            throw new ArgumentException("The PDF contains no pages.");
        }

        var fullText = string.Join("\n\n", pageTexts);
        var academicTerm = ClassListPdfClassHeaderParser.TryParseAcademicTerm(fullText) ?? "Unspecified";
        var included = includedRowKeys == null
            ? null
            : new HashSet<string>(includedRowKeys.Where(k => !string.IsNullOrWhiteSpace(k)), StringComparer.Ordinal);

        var catalog = (await _repository.GetAllAsync(null, cancellationToken)).ToList();
        var pageResults = new List<ClassRosterPdfImportPageResult>();

        for (var i = 0; i < pageTexts.Count; i++)
        {
            var pageText = pageTexts[i];
            var pageNum = i + 1;

            var parsed = ClassListPdfParser.ParseText(pageText);
            if (parsed.Count == 0)
            {
                pageResults.Add(new ClassRosterPdfImportPageResult
                {
                    PageNumber = pageNum,
                    SkippedReason = "No student rows on this page."
                });
                continue;
            }

            if (included != null)
            {
                parsed = parsed
                    .Where(r => included.Contains(BuildPreviewRowKey(pageNum, r.StudentNumber)))
                    .ToList();
                if (parsed.Count == 0)
                {
                    pageResults.Add(new ClassRosterPdfImportPageResult
                    {
                        PageNumber = pageNum,
                        SkippedReason = "No selected student rows on this page."
                    });
                    continue;
                }
            }

            if (!ClassListPdfClassHeaderParser.TryParseClassHeaderFromPage(pageText, out var header))
            {
                pageResults.Add(new ClassRosterPdfImportPageResult
                {
                    PageNumber = pageNum,
                    SkippedReason =
                        "Could not read the course/class line on this page. Use the official STI class list PDF export."
                });
                continue;
            }

            var yearLevel = ClassListPdfParser.MajorityYearLevel(parsed);
            if (string.IsNullOrEmpty(yearLevel))
            {
                pageResults.Add(new ClassRosterPdfImportPageResult
                {
                    PageNumber = pageNum,
                    SkippedReason = "Could not determine year level from student rows on this page."
                });
                continue;
            }

            var match = FindMatchingFacultyClass(catalog, header, yearLevel);
            await using var tx = await _unitOfWork.BeginTransactionAsync(cancellationToken);
            FacultyClass target;
            var created = false;
            if (match != null)
            {
                target = match;
            }
            else
            {
                var entity = FacultyClass.Create(
                    header.CourseCode.Trim(),
                    header.ClassNumber.Trim(),
                    header.SectionLetter.Trim(),
                    header.CourseTitle.Trim(),
                    "LEC",
                    academicTerm.Trim(),
                    header.EnrolledFromHeader ?? 0,
                    header.ProgramCode.Trim(),
                    yearLevel.Trim());

                target = await _repository.CreateAsync(entity, cancellationToken);
                catalog.Add(target);
                created = true;
            }

            var prepared = await PrepareEnrollmentsAsync(target.id, parsed, cancellationToken);
            await _enrollmentRepository.ReplaceAllForClassAsync(target.id, prepared.Enrollments, cancellationToken);
            await _repository.UpdateEnrolledCountAsync(target.id, prepared.Enrollments.Count, cancellationToken);
            await tx.CommitAsync(cancellationToken);

            pageResults.Add(new ClassRosterPdfImportPageResult
            {
                PageNumber = pageNum,
                CourseCode = header.CourseCode,
                ClassNumber = header.ClassNumber,
                Section = header.SectionLetter,
                ProgramCode = header.ProgramCode,
                YearLevel = yearLevel,
                FacultyClassId = target.id,
                ClassCreatedFromPdf = created,
                ImportedCount = prepared.Enrollments.Count,
                AutoCreatedCount = prepared.AutoCreatedCount,
                NotFoundInRegistry = prepared.NotFoundInRegistry,
                Warnings = prepared.Warnings
            });
        }

        return new ClassRosterPdfImportSummaryResponse { Pages = pageResults };
    }

    public async Task<ClassListPdfPreviewResponse> PreviewClassListPdfAsync(
        Stream pdfStream,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pdfStream);

        IReadOnlyList<string> pageTexts;
        try
        {
            pageTexts = ClassListPdfParser.ExtractPageTexts(pdfStream);
        }
        catch (Exception ex)
        {
            throw new ArgumentException("Could not read the PDF. Ensure it is a valid PDF file.", ex);
        }

        if (pageTexts.Count == 0)
        {
            throw new ArgumentException("The PDF contains no pages.");
        }

        var fullText = string.Join("\n\n", pageTexts);
        var academicTerm = ClassListPdfClassHeaderParser.TryParseAcademicTerm(fullText) ?? "Unspecified";

        var pageResults = new List<ClassListPdfPreviewPage>();

        for (var i = 0; i < pageTexts.Count; i++)
        {
            var pageText = pageTexts[i];
            var pageNum = i + 1;

            var parsed = ClassListPdfParser.ParseText(pageText);
            if (parsed.Count == 0)
            {
                pageResults.Add(new ClassListPdfPreviewPage
                {
                    PageNumber = pageNum,
                    AcademicTerm = academicTerm,
                    SkippedReason = "No student rows on this page.",
                    CourseExistsInModule = false
                });
                continue;
            }

            if (!ClassListPdfClassHeaderParser.TryParseClassHeaderFromPage(pageText, out var header))
            {
                pageResults.Add(new ClassListPdfPreviewPage
                {
                    PageNumber = pageNum,
                    AcademicTerm = academicTerm,
                    SkippedReason =
                        "Could not read the course/class line on this page. Use the official STI class list PDF export.",
                    CourseExistsInModule = false
                });
                continue;
            }

            var yearLevel = ClassListPdfParser.MajorityYearLevel(parsed);
            if (string.IsNullOrEmpty(yearLevel))
            {
                pageResults.Add(new ClassListPdfPreviewPage
                {
                    PageNumber = pageNum,
                    AcademicTerm = academicTerm,
                    SkippedReason = "Could not determine year level from student rows on this page.",
                    CourseExistsInModule = false
                });
                continue;
            }

            var courseRow = await _courseService.GetCourseByCodeAsync(header.CourseCode.Trim());
            var exists = courseRow != null;

            pageResults.Add(new ClassListPdfPreviewPage
            {
                PageNumber = pageNum,
                AcademicTerm = academicTerm,
                SkippedReason = null,
                CourseCode = header.CourseCode.Trim(),
                CourseTitle = header.CourseTitle.Trim(),
                TotalUnits = header.TotalUnits,
                ProgramCode = header.ProgramCode.Trim(),
                ClassNumber = header.ClassNumber.Trim(),
                SectionLetter = header.SectionLetter.Trim(),
                YearLevel = yearLevel.Trim(),
                CourseExistsInModule = exists,
                Students = parsed
                    .Select(r => new ClassListPdfPreviewStudentRow
                    {
                        RowKey = BuildPreviewRowKey(pageNum, r.StudentNumber),
                        StudentNumber = r.StudentNumber.Trim(),
                        DisplayName = r.DisplayName.Trim(),
                        ProgramCode = r.ProgramCode.Trim(),
                        YearLevel = r.YearLevel.Trim()
                    })
                    .ToList()
            });
        }

        return new ClassListPdfPreviewResponse
        {
            AcademicTerm = academicTerm,
            Pages = pageResults
        };
    }

    private static FacultyClass? FindMatchingFacultyClass(
        List<FacultyClass> catalog,
        ParsedClassHeader header,
        string yearLevel)
    {
        foreach (var c in catalog)
        {
            if (!FieldMatch(c.course_code, header.CourseCode))
            {
                continue;
            }

            if (!FieldMatch(c.class_number, header.ClassNumber))
            {
                continue;
            }

            if (!FieldMatch(c.section, header.SectionLetter))
            {
                continue;
            }

            if (!FieldMatch(c.program_code, header.ProgramCode))
            {
                continue;
            }

            if (!YearLevelMatch(c.year_level, yearLevel))
            {
                continue;
            }

            return c;
        }

        return null;
    }

    private static bool YearLevelMatch(string? dbValue, string pdfValue)
    {
        if (FieldMatch(dbValue, pdfValue))
        {
            return true;
        }

        var p = pdfValue.Trim();
        var d = dbValue?.Trim() ?? string.Empty;
        var ym = Regex.Match(p, @"^(\d+)Y", RegexOptions.IgnoreCase);
        if (!ym.Success)
        {
            return false;
        }

        var n = ym.Groups[1].Value;
        return d.Equals($"Year {n}", StringComparison.OrdinalIgnoreCase);
    }

    private static bool FieldMatch(string? dbValue, string pdfValue)
    {
        return string.Equals(dbValue?.Trim(), pdfValue.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    private static string BuildPreviewRowKey(int pageNumber, string studentNumber)
    {
        return $"{pageNumber}:{studentNumber.Trim()}";
    }

    private sealed class PreparedEnrollments
    {
        public List<FacultyClassEnrollment> Enrollments { get; } = new();
        public List<string> Warnings { get; } = new();
        public List<RosterPdfStudentNotInRegistry> NotFoundInRegistry { get; } = new();
        public int AutoCreatedCount { get; set; }
    }

    private async Task<PreparedEnrollments> PrepareEnrollmentsAsync(
        long facultyClassId,
        IReadOnlyList<ClassListPdfParser.ParsedRow> parsed,
        CancellationToken cancellationToken)
    {
        var distinctNumbers = new List<string>();
        var distinctSeen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var row in parsed)
        {
            var key = row.StudentNumber.Trim();
            if (key.Length == 0)
            {
                continue;
            }

            if (distinctSeen.Add(key))
            {
                distinctNumbers.Add(key);
            }
        }

        var existingStudents = await _studentRepository.GetActiveStudentsByStudentNumbersAsync(
            distinctNumbers,
            cancellationToken);
        var studentsByNumber = new Dictionary<string, Student>(existingStudents, StringComparer.Ordinal);

        var result = new PreparedEnrollments();
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var row in parsed)
        {
            var key = row.StudentNumber.Trim();
            if (key.Length == 0)
            {
                result.Warnings.Add("Skipped a row with an empty student number.");
                continue;
            }

            if (!seen.Add(key))
            {
                result.Warnings.Add($"Duplicate student number skipped: {key}");
                continue;
            }

            if (!studentsByNumber.TryGetValue(key, out var student))
            {
                var autoCreated = await TryAutoCreateStudentFromPdfRowAsync(row, cancellationToken);
                if (autoCreated != null)
                {
                    studentsByNumber[key] = autoCreated;
                    student = autoCreated;
                    result.AutoCreatedCount++;
                }
                else
                {
                    result.NotFoundInRegistry.Add(new RosterPdfStudentNotInRegistry
                    {
                        StudentNumber = key,
                        PdfDisplayName = row.DisplayName.Trim(),
                        PdfProgramCode = row.ProgramCode.Trim(),
                        PdfYearLevel = row.YearLevel.Trim()
                    });
                    result.Warnings.Add($"Skipped student {key}: could not auto-create from PDF row.");
                    continue;
                }
            }

            result.Enrollments.Add(FacultyClassEnrollment.Create(facultyClassId, student.id));
        }

        return result;
    }

    private async Task ApplyRosterReplaceInTransactionAsync(
        long facultyClassId,
        IReadOnlyList<FacultyClassEnrollment> enrollments,
        CancellationToken cancellationToken)
    {
        await using var tx = await _unitOfWork.BeginTransactionAsync(cancellationToken);
        await _enrollmentRepository.ReplaceAllForClassAsync(facultyClassId, enrollments, cancellationToken);
        await _repository.UpdateEnrolledCountAsync(facultyClassId, enrollments.Count, cancellationToken);
        await tx.CommitAsync(cancellationToken);
    }

    private async Task<Student?> TryAutoCreateStudentFromPdfRowAsync(
        ClassListPdfParser.ParsedRow row,
        CancellationToken cancellationToken)
    {
        var studentNumber = row.StudentNumber.Trim();
        if (studentNumber.Length == 0)
        {
            return null;
        }

        var (firstName, lastName, middleName) = ParsePdfDisplayName(row.DisplayName);
        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
        {
            return null;
        }

        var programCode = row.ProgramCode.Trim();
        if (programCode.Length == 0)
        {
            return null;
        }

        var yearLevel = row.YearLevel.Trim();
        if (yearLevel.Length == 0)
        {
            return null;
        }

        var existing = await _studentRepository.GetStudentByStudentNumberAsync(studentNumber, cancellationToken);
        if (existing != null)
        {
            return existing;
        }

        var programTitle = programCode;
        var program = await _programRepository.GetProgramByCodeAsync(programCode);
        if (!string.IsNullOrWhiteSpace(program?.program_title))
        {
            programTitle = program.program_title.Trim();
        }

        var created = Student.Create(
            studentNumber,
            firstName,
            lastName,
            middleName,
            programCode,
            programTitle,
            yearLevel,
            studentType: "Regular",
            enrollmentStatus: "Active");

        return await _studentRepository.CreateStudentAsync(created);
    }

    private static (string FirstName, string LastName, string? MiddleName) ParsePdfDisplayName(string displayName)
    {
        var text = displayName.Trim();
        if (string.IsNullOrWhiteSpace(text))
        {
            return (string.Empty, string.Empty, null);
        }

        var commaIndex = text.IndexOf(',');
        if (commaIndex >= 0)
        {
            var last = text[..commaIndex].Trim();
            var rest = text[(commaIndex + 1)..].Trim();
            var parts = rest
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var first = parts.Length > 0 ? parts[0] : string.Empty;
            var middle = parts.Length > 1 ? string.Join(" ", parts.Skip(1)) : null;
            return (first, last, string.IsNullOrWhiteSpace(middle) ? null : middle);
        }

        var tokens = text
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (tokens.Length == 0)
        {
            return (string.Empty, string.Empty, null);
        }

        if (tokens.Length == 1)
        {
            return (tokens[0], tokens[0], null);
        }

        var lastName = tokens[^1];
        var firstName = tokens[0];
        var middleName = tokens.Length > 2 ? string.Join(" ", tokens.Skip(1).Take(tokens.Length - 2)) : null;
        return (firstName, lastName, string.IsNullOrWhiteSpace(middleName) ? null : middleName);
    }

    private static ClassRosterResponse MapToResponse(FacultyClass e)
    {
        return new ClassRosterResponse
        {
            Id = e.id,
            CourseCode = e.course_code,
            ClassNumber = e.class_number,
            Section = e.section,
            CourseTitle = e.course_title,
            Component = e.component,
            AcademicTerm = e.academic_term,
            Enrolled = e.enrolled_count,
            ProgramCode = e.program_code,
            YearLevel = e.year_level
        };
    }

    private static IReadOnlyList<GradeRosterClassLookupResponse> MapToGradeRosterClassLookup(IReadOnlyList<FacultyClass> rows)
    {
        if (rows.Count == 0)
        {
            return Array.Empty<GradeRosterClassLookupResponse>();
        }

        var baseTexts = rows
            .Select(e => $"{e.course_code.Trim()}-{e.course_title.Trim()}")
            .ToList();
        var duplicateBase = baseTexts
            .GroupBy(t => t)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToHashSet(StringComparer.Ordinal);

        return rows.Select(e =>
        {
            var code = e.course_code.Trim();
            var title = e.course_title.Trim();
            var baseText = $"{code}-{title}";
            var display = baseText;
            if (duplicateBase.Contains(baseText))
            {
                var sec = e.section.Trim();
                display = string.IsNullOrEmpty(sec) ? baseText : $"{baseText}-{sec}";
            }

            return new GradeRosterClassLookupResponse
            {
                Id = e.id,
                CourseCode = code,
                CourseTitle = title,
                DisplayText = display
            };
        }).ToList();
    }
}


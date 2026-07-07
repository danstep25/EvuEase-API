using AutoMapper;
using EvuEase.Application.Common;
using EvuEase.Application.Curriculum;
using EvuEase.Application.DTOs.Course;
using EvuEase.Application.Interfaces.Persistence;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Application.Interfaces.Services;
using EvuEase.Domain.Entities;

namespace EvuEase.Application.Services;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly ICurriculaRepository _curriculaRepository;
    private readonly IProgramRepository _programRepository;
    private readonly IApplicationUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CourseService(
        ICourseRepository courseRepository,
        ICurriculaRepository curriculaRepository,
        IProgramRepository programRepository,
        IApplicationUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _courseRepository = courseRepository;
        _curriculaRepository = curriculaRepository;
        _programRepository = programRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResults<CourseResponse>> GetAllCourses(CourseRequest courseRequest)
    {
        var pagedEntities = await _courseRepository.GetAllCourses(courseRequest);
        var courseList = pagedEntities.Result.ToList();
        
        var responseList = new List<CourseResponse>();
        foreach (var course in courseList)
        {
            var response = await MapCourseToResponse(course);
            responseList.Add(response);
        }
        
        return new PagedResults<CourseResponse>(
            pagedEntities.PageIndex,
            pagedEntities.PageSize,
            pagedEntities.TotalRecords,
            pagedEntities.TotalEntries,
            responseList
        );
    }

    public async Task<CourseResponse?> GetCourseByCodeAsync(string courseCode)
    {
        var course = await _courseRepository.GetCourseByCodeAsync(courseCode);
        if (course == null)
        {
            return null;
        }
        
        return await MapCourseToResponse(course);
    }

    public async Task<CourseResponse> CreateCourseAsync(CreateCourseRequest courseRequest)
    {
        ValidateCourseTitle(courseRequest.CourseTitle);

        var program = await _programRepository.GetProgramByIdAsync(courseRequest.ProgramId);
        if (program == null)
        {
            throw new Exception($"Program with ID '{courseRequest.ProgramId}' not found.");
        }

        var curriculum = await _curriculaRepository.GetCurriculaByCodeAsync(courseRequest.CurriculumCode);
        if (curriculum == null)
        {
            throw new Exception($"Curriculum with code '{courseRequest.CurriculumCode}' not found.");
        }

        if (curriculum.program_id != courseRequest.ProgramId)
        {
            throw new Exception($"Curriculum '{courseRequest.CurriculumCode}' does not belong to program ID '{courseRequest.ProgramId}'.");
        }

        var existingCourse = await _courseRepository.GetCourseByCodeInCurriculumAsync(
            courseRequest.CourseCode,
            curriculum.id);
        if (existingCourse != null)
        {
            throw new Exception(
                $"Course with code '{courseRequest.CourseCode}' already exists in curriculum '{courseRequest.CurriculumCode}'.");
        }

        var isElectiveSlot = ElectiveSubjectHelper.ResolveIsElectiveSlot(
            courseRequest.CourseTitle,
            courseRequest.CourseCode,
            courseRequest.IsElectiveSlot);
        var isElectiveOption = ElectiveSubjectHelper.ResolveIsElectiveOption(
            isElectiveSlot,
            courseRequest.IsElectiveOption);

        var normalizedPrerequisites = await ValidateCoursePrerequisitesAsync(
            courseRequest.Prerequisites,
            courseRequest.CourseCode,
            curriculum.id);

        var course = Course.Create(
            courseRequest.CourseCode,
            curriculum.id,
            courseRequest.ProgramId,
            courseRequest.CourseTitle,
            courseRequest.CourseTotalUnits,
            courseRequest.CourseYearLevel,
            courseRequest.CourseSemester,
            courseRequest.CourseComponent,
            normalizedPrerequisites,
            courseRequest.Description,
            courseRequest.CourseLecUnits,
            courseRequest.CourseLabUnits,
            isElectiveSlot,
            isElectiveOption
        );

        try
        {
            var result = await _courseRepository.CreateCourseAsync(course);
            return await MapCourseToResponse(result);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to create course: {ex.Message}. Inner exception: {ex.InnerException?.Message}", ex);
        }
    }

    public async Task<CourseResponse> UpdateCourseAsync(string courseCode, UpdateCourseRequest courseRequest)
    {
        ValidateCourseTitle(courseRequest.CourseTitle);

        var program = await _programRepository.GetProgramByIdAsync(courseRequest.ProgramId);
        if (program == null)
        {
            throw new Exception($"Program with ID '{courseRequest.ProgramId}' not found.");
        }

        var curriculum = await _curriculaRepository.GetCurriculaByCodeAsync(courseRequest.CurriculumCode);
        if (curriculum == null)
        {
            throw new Exception($"Curriculum with code '{courseRequest.CurriculumCode}' not found.");
        }

        if (curriculum.program_id != courseRequest.ProgramId)
        {
            throw new Exception($"Curriculum '{courseRequest.CurriculumCode}' does not belong to program ID '{courseRequest.ProgramId}'.");
        }

        var course = await _courseRepository.GetCourseByCodeInCurriculumAsync(courseCode, curriculum.id);
        if (course == null)
        {
            throw new Exception(
                $"Course with code '{courseCode}' was not found in curriculum '{courseRequest.CurriculumCode}'.");
        }

        var isElectiveSlot = ElectiveSubjectHelper.ResolveIsElectiveSlot(
            courseRequest.CourseTitle,
            course.course_code,
            courseRequest.IsElectiveSlot);
        var isElectiveOption = ElectiveSubjectHelper.ResolveIsElectiveOption(
            isElectiveSlot,
            courseRequest.IsElectiveOption);

        var normalizedPrerequisites = await ValidateCoursePrerequisitesAsync(
            courseRequest.Prerequisites,
            course.course_code,
            curriculum.id);

        course.Update(
            curriculum.id,
            courseRequest.ProgramId,
            courseRequest.CourseTitle,
            courseRequest.CourseTotalUnits,
            courseRequest.CourseYearLevel,
            courseRequest.CourseSemester,
            courseRequest.CourseComponent,
            normalizedPrerequisites,
            courseRequest.Description,
            course.course_lec_units,
            course.course_lab_units,
            isElectiveSlot,
            isElectiveOption
        );

        try
        {
            var result = await _courseRepository.UpdateCourseAsync(course);
            return await MapCourseToResponse(result);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to update course: {ex.Message}. Inner exception: {ex.InnerException?.Message}", ex);
        }
    }

    public async Task DeleteCourseAsync(string courseCode, string curriculumCode)
    {
        var normalizedCurriculumCode = curriculumCode?.Trim() ?? string.Empty;
        if (normalizedCurriculumCode.Length == 0)
        {
            throw new InvalidOperationException("Curriculum code is required to delete a course.");
        }

        var curriculum = await _curriculaRepository.GetCurriculaByCodeAsync(normalizedCurriculumCode);
        if (curriculum == null)
        {
            throw new Exception($"Curriculum with code '{normalizedCurriculumCode}' not found.");
        }

        var course = await _courseRepository.GetCourseByCodeInCurriculumAsync(courseCode, curriculum.id);
        if (course == null)
        {
            throw new Exception(
                $"Course with code '{courseCode}' was not found in curriculum '{normalizedCurriculumCode}'.");
        }

        await _courseRepository.DeleteCourseAsync(course);
    }

    public async Task<CourseBatchImportPreviewResponse> PreviewBatchImportAsync(
        CourseBatchImportRequest request,
        CancellationToken cancellationToken = default)
    {
        var (curriculum, program, rows) = await ValidateBatchContextAsync(request, cancellationToken);
        return await BuildBatchPreviewResponse(request, curriculum, program, rows);
    }

    public async Task<CourseBatchImportPreviewResponse> PreviewBatchPdfAsync(
        Stream pdfStream,
        long programId,
        string curriculumCode,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pdfStream);

        CurriculumStructurePdfParser.ParseResult parsed;
        try
        {
            parsed = CurriculumStructurePdfParser.Parse(pdfStream);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Could not read the PDF. Ensure it is a valid curriculum structure PDF.", ex);
        }

        if (parsed.Rows.Count == 0)
        {
            var detail = parsed.Warnings.Count > 0
                ? string.Join(" ", parsed.Warnings)
                : parsed.UsedOcr
                    ? "OCR could not recover course rows from this scanned PDF. Try a clearer scan or the original text-based export."
                    : "No course rows were found in the PDF.";
            throw new InvalidOperationException(detail);
        }

        var request = new CourseBatchImportRequest
        {
            ProgramId = programId,
            CurriculumCode = curriculumCode,
            Rows = parsed.Rows.Select(row => new CourseBatchImportRowDto
            {
                RowNumber = row.SourceLineNumber,
                CourseCode = row.CourseCode,
                CourseTitle = row.CourseTitle,
                CourseLecUnits = row.LecUnits,
                CourseLabUnits = row.LabUnits,
                CourseTotalUnits = row.TotalUnits,
                CourseYearLevel = row.YearLevel,
                CourseSemester = row.Semester,
                Prerequisites = row.Prerequisites,
                CourseComponent = CourseBatchImportHelper.DeriveComponent(row.LecUnits, row.LabUnits),
                Selected = true
            }).ToList()
        };

        var preview = await PreviewBatchImportAsync(request, cancellationToken);
        preview.SkippedPdfLines = parsed.SkippedLines;
        preview.ParseWarnings = parsed.Warnings;
        preview.DetectedReferenceNumber = parsed.Reference?.ReferenceNumber;

        return preview;
    }

    public async Task<CourseBatchPdfDetectionResponse> DetectBatchPdfAsync(
        Stream pdfStream,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pdfStream);

        string text;
        try
        {
            text = CurriculumStructurePdfParser.ExtractFullTextAsString(pdfStream);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Could not read the PDF. Ensure it is a valid curriculum structure PDF.", ex);
        }

        var reference = CurriculumStructurePdfParser.ExtractReferenceMetadata(text);
        if (reference == null)
        {
            return new CourseBatchPdfDetectionResponse();
        }

        var response = new CourseBatchPdfDetectionResponse
        {
            ReferenceNumber = reference.ReferenceNumber,
            ProgramCode = reference.ProgramCode,
            CurriculumCode = reference.CurriculumCode,
            SchoolYear = reference.SchoolYear
        };

        var curriculum = await _curriculaRepository.GetCurriculaByCodeAsync(reference.CurriculumCode);
        if (curriculum != null)
        {
            response.CurriculumFound = true;
            response.ProgramId = curriculum.program_id;
            return response;
        }

        var program = await _programRepository.GetProgramByCodeAsync(reference.ProgramCode);
        if (program != null)
        {
            response.ProgramId = program.program_id;
        }

        return response;
    }

    public async Task<CourseBatchImportResultResponse> ImportBatchAsync(
        CourseBatchImportRequest request,
        CancellationToken cancellationToken = default)
    {
        var preview = await PreviewBatchImportAsync(request, cancellationToken);
        var importable = preview.Rows
            .Where(r => r.Selected
                        && !string.Equals(r.Status, "Error", StringComparison.OrdinalIgnoreCase)
                        && r.CourseTitle.Length <= CourseValidationConstants.MaxTitleLength)
            .ToList();

        if (importable.Count == 0)
        {
            throw new InvalidOperationException("No valid rows selected for import.");
        }

        var curriculum = await _curriculaRepository.GetCurriculaByCodeAsync(request.CurriculumCode.Trim());
        if (curriculum == null)
        {
            throw new InvalidOperationException($"Curriculum '{request.CurriculumCode}' was not found.");
        }

        var result = new CourseBatchImportResultResponse
        {
            SkippedCount = preview.Rows.Count - importable.Count
        };

        await using var tx = await _unitOfWork.BeginTransactionAsync(cancellationToken);
        foreach (var row in importable)
        {
            var isElectiveSlot = ElectiveSubjectHelper.IsElectiveSlot(row.CourseTitle, row.CourseCode);
            var course = Course.Create(
                row.CourseCode,
                curriculum.id,
                request.ProgramId,
                row.CourseTitle,
                row.CourseTotalUnits,
                row.CourseYearLevel,
                row.CourseSemester,
                row.CourseComponent,
                row.Prerequisites,
                description: row.CourseTitle,
                row.CourseLecUnits,
                row.CourseLabUnits,
                isElectiveSlot,
                isElectiveOption: false);

            await _courseRepository.CreateCourseAsync(course);
            result.ImportedCount++;
        }

        await tx.CommitAsync(cancellationToken);

        result.Warnings.AddRange(
            preview.Rows
                .Where(r => r.Selected && r.Messages.Count > 0 && !string.Equals(r.Status, "Error", StringComparison.OrdinalIgnoreCase))
                .SelectMany(r => r.Messages.Select(m => $"Row {r.RowNumber}: {m}")));

        return result;
    }

    private async Task<(Curricula curriculum, Domain.Entities.Program program, List<CourseBatchImportPreviewRowDto> rows)> ValidateBatchContextAsync(
        CourseBatchImportRequest request,
        CancellationToken cancellationToken)
    {
        if (request.ProgramId <= 0)
        {
            throw new InvalidOperationException("Program is required.");
        }

        var curriculumCode = request.CurriculumCode?.Trim() ?? string.Empty;
        if (curriculumCode.Length == 0)
        {
            throw new InvalidOperationException("Curriculum is required.");
        }

        if (request.Rows == null || request.Rows.Count == 0)
        {
            throw new InvalidOperationException("At least one course row is required.");
        }

        var program = await _programRepository.GetProgramByIdAsync(request.ProgramId);
        if (program == null)
        {
            throw new InvalidOperationException($"Program with ID '{request.ProgramId}' was not found.");
        }

        var curriculum = await _curriculaRepository.GetCurriculaByCodeAsync(curriculumCode);
        if (curriculum == null)
        {
            throw new InvalidOperationException($"Curriculum '{curriculumCode}' was not found.");
        }

        if (curriculum.program_id != request.ProgramId)
        {
            throw new InvalidOperationException(
                $"Curriculum '{curriculumCode}' does not belong to the selected program.");
        }

        var rows = request.Rows
            .Select(NormalizeBatchRow)
            .Where(r => !CourseBatchImportHelper.IsSkippableRow(r.CourseCode, r.CourseTitle))
            .ToList();

        if (rows.Count == 0)
        {
            throw new InvalidOperationException("No importable course rows were found after parsing.");
        }

        return (curriculum, program, rows);
    }

    private static CourseBatchImportPreviewRowDto NormalizeBatchRow(CourseBatchImportRowDto row)
    {
        var lec = Math.Max(0, row.CourseLecUnits);
        var lab = Math.Max(0, row.CourseLabUnits);
        var total = row.CourseTotalUnits > 0 ? row.CourseTotalUnits : lec + lab;
        var prerequisites = CourseBatchImportHelper.NormalizePrerequisites(row.Prerequisites);

        return new CourseBatchImportPreviewRowDto
        {
            RowNumber = row.RowNumber,
            CourseCode = row.CourseCode.Trim().ToUpperInvariant(),
            CourseTitle = row.CourseTitle.Trim(),
            CourseLecUnits = lec,
            CourseLabUnits = lab,
            CourseTotalUnits = total,
            CourseYearLevel = CourseBatchImportHelper.NormalizeYearLevel(row.CourseYearLevel),
            CourseSemester = CourseBatchImportHelper.NormalizeSemester(row.CourseSemester),
            Prerequisites = prerequisites,
            CourseComponent = row.CourseComponent?.Trim()
                ?? CourseBatchImportHelper.DeriveComponent(lec, lab),
            IsElectiveSlot = ElectiveSubjectHelper.IsElectiveSlot(row.CourseTitle, row.CourseCode),
            Selected = row.Selected
        };
    }

    private async Task<CourseBatchImportPreviewResponse> BuildBatchPreviewResponse(
        CourseBatchImportRequest request,
        Curricula curriculum,
        Domain.Entities.Program program,
        List<CourseBatchImportPreviewRowDto> rows)
    {
        var batchCodes = rows
            .Select(r => r.CourseCode)
            .Where(c => c.Length > 0)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var prereqCodes = rows
            .SelectMany(r => CourseBatchImportHelper.ParsePrerequisiteCodes(r.Prerequisites))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var lookupCodes = batchCodes.Union(prereqCodes).ToList();
        var existingCodesInCurriculum = await _courseRepository.GetExistingCourseCodesForCurriculumAsync(
            curriculum.id,
            lookupCodes);

        var duplicateTracker = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var row in rows)
        {
            ValidateBatchPreviewRow(
                row,
                duplicateTracker,
                existingCodesInCurriculum,
                batchCodes,
                curriculum.curriculum_code);
        }

        return new CourseBatchImportPreviewResponse
        {
            CurriculumCode = curriculum.curriculum_code,
            ProgramCode = program.program_code,
            TotalRows = rows.Count,
            ValidRows = rows.Count(r => string.Equals(r.Status, "Valid", StringComparison.OrdinalIgnoreCase)),
            WarningRows = rows.Count(r => string.Equals(r.Status, "Warning", StringComparison.OrdinalIgnoreCase)),
            ErrorRows = rows.Count(r => string.Equals(r.Status, "Error", StringComparison.OrdinalIgnoreCase)),
            Rows = rows
        };
    }

    private static void ValidateBatchPreviewRow(
        CourseBatchImportPreviewRowDto row,
        HashSet<string> duplicateTracker,
        HashSet<string> existingCodesInCurriculum,
        HashSet<string> batchCodes,
        string curriculumCode)
    {
        var messages = row.Messages;

        if (row.CourseCode.Length == 0)
        {
            messages.Add("Course code is required.");
        }
        else if (row.CourseCode.Length > 20)
        {
            messages.Add("Course code must be 20 characters or fewer.");
        }

        if (row.CourseTitle.Length == 0)
        {
            messages.Add("Course description is required.");
        }
        else if (row.CourseTitle.Length > CourseValidationConstants.MaxTitleLength)
        {
            messages.Add(CourseValidationConstants.TitleTooLongMessage);
        }

        if (row.CourseYearLevel.Length == 0)
        {
            messages.Add("Year level is required.");
        }

        if (row.CourseSemester.Length == 0)
        {
            messages.Add("Semester is required.");
        }

        if (row.CourseTotalUnits <= 0)
        {
            messages.Add("Total units must be greater than zero.");
        }
        else if (row.CourseLecUnits + row.CourseLabUnits > 0
                 && row.CourseTotalUnits != row.CourseLecUnits + row.CourseLabUnits)
        {
            messages.Add("Total units does not match LEC + LAB.");
        }

        if (row.Prerequisites != null && row.Prerequisites.Length > CourseValidationConstants.MaxPrerequisitesLength)
        {
            messages.Add(CourseValidationConstants.PrerequisitesTooLongMessage);
        }

        if (row.CourseCode.Length > 0)
        {
            if (!duplicateTracker.Add(row.CourseCode))
            {
                messages.Add($"Duplicate course code in file: {row.CourseCode}.");
            }

            if (existingCodesInCurriculum.Contains(row.CourseCode))
            {
                messages.Add(
                    $"Course code '{row.CourseCode}' already exists in curriculum '{curriculumCode}'.");
            }
        }

        foreach (var prereq in CourseBatchImportHelper.ParsePrerequisiteCodes(row.Prerequisites))
        {
            if (string.Equals(prereq, row.CourseCode, StringComparison.OrdinalIgnoreCase))
            {
                messages.Add($"Course cannot list itself as a pre-requisite ({prereq}).");
                continue;
            }

            if (!batchCodes.Contains(prereq) && !existingCodesInCurriculum.Contains(prereq))
            {
                messages.Add(
                    $"Pre-requisite '{prereq}' was not found in this file or in curriculum '{curriculumCode}'.");
            }
        }

        row.Status = messages.Any(m => IsBatchErrorMessage(m))
            ? "Error"
            : messages.Count > 0
                ? "Warning"
                : "Valid";
    }

    private static bool IsBatchErrorMessage(string message)
    {
        if (CourseValidationConstants.IsTitleLengthMessage(message))
        {
            return false;
        }

        return message.Contains("required", StringComparison.OrdinalIgnoreCase)
               || message.Contains("already exists", StringComparison.OrdinalIgnoreCase)
               || message.Contains("Duplicate", StringComparison.OrdinalIgnoreCase)
               || message.Contains("must be", StringComparison.OrdinalIgnoreCase)
               || message.Contains("cannot list", StringComparison.OrdinalIgnoreCase)
               || message.Contains("not found", StringComparison.OrdinalIgnoreCase)
               || message.Contains("exceed", StringComparison.OrdinalIgnoreCase);
    }

    private static void ValidateCourseTitle(string? title)
    {
        var trimmed = title?.Trim() ?? string.Empty;
        if (trimmed.Length == 0)
        {
            throw new InvalidOperationException("Course title is required.");
        }

        if (trimmed.Length > CourseValidationConstants.MaxTitleLength)
        {
            throw new InvalidOperationException(CourseValidationConstants.TitleTooLongMessage);
        }
    }

    private async Task<string?> ValidateCoursePrerequisitesAsync(
        string? rawPrerequisites,
        string courseCode,
        long curriculumId)
    {
        var normalized = CourseBatchImportHelper.NormalizePrerequisites(rawPrerequisites);

        if (normalized != null && normalized.Length > CourseValidationConstants.MaxPrerequisitesLength)
        {
            throw new InvalidOperationException(CourseValidationConstants.PrerequisitesTooLongMessage);
        }

        var codes = CourseBatchImportHelper.ParsePrerequisiteCodes(normalized);
        if (codes.Count == 0)
        {
            return normalized;
        }

        foreach (var prereq in codes)
        {
            if (string.Equals(prereq, courseCode, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"Course cannot list itself as a pre-requisite ({prereq}).");
            }
        }

        var existingCodes = await _courseRepository.GetExistingCourseCodesForCurriculumAsync(curriculumId, codes);
        foreach (var prereq in codes)
        {
            if (!existingCodes.Contains(prereq))
            {
                throw new InvalidOperationException($"Pre-requisite '{prereq}' was not found in this curriculum.");
            }
        }

        return normalized;
    }

    private async Task<CourseResponse> MapCourseToResponse(Course course)
    {
        var response = new CourseResponse
        {
            CourseCode = course.course_code,
            ProgramId = course.program_id,
            CourseTitle = course.course_title,
            CourseLecUnits = course.course_lec_units,
            CourseLabUnits = course.course_lab_units,
            CourseTotalUnits = course.course_total_units,
            CourseYearLevel = course.course_yearlevel,
            CourseSemester = course.course_semester,
            CourseComponent = course.course_component,
            Prerequisites = course.prerequisites,
            Description = course.description,
            CourseHasPrerequisites = course.course_has_prerequities,
            IsElectiveSlot = course.is_elective_slot,
            IsElectiveOption = course.is_elective_option,
            Status = course.status ? "Active" : "Inactive",
            CreatedAt = course.created_at,
            UpdatedAt = course.updated_at
        };

        var curriculum = await _curriculaRepository.GetCurriculaByIdAsync(course.curriculum_id);
        if (curriculum != null)
        {
            response.CurriculumCode = curriculum.curriculum_code;
        }

        var program = await _programRepository.GetProgramByIdAsync(course.program_id);
        if (program != null)
        {
            response.ProgramCode = program.program_code;
            response.ProgramTitle = program.program_title;
        }

        return response;
    }
}


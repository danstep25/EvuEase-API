using EvuEase.Application.Common;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Application.Interfaces.Services;
using EvuEase.Domain.Entities;

namespace EvuEase.Application.Services;

public class StudentCurriculumAssignmentService : IStudentCurriculumAssignmentService
{
    public const string DefaultFirstYearReason =
        "Auto-assigned current active curriculum for first year student.";

    private readonly IStudentRepository _studentRepository;
    private readonly IProgramRepository _programRepository;
    private readonly ICurriculaRepository _curriculaRepository;
    private readonly IStudentCurriculumHistoryRepository _curriculumHistoryRepository;

    public StudentCurriculumAssignmentService(
        IStudentRepository studentRepository,
        IProgramRepository programRepository,
        ICurriculaRepository curriculaRepository,
        IStudentCurriculumHistoryRepository curriculumHistoryRepository)
    {
        _studentRepository = studentRepository;
        _programRepository = programRepository;
        _curriculaRepository = curriculaRepository;
        _curriculumHistoryRepository = curriculumHistoryRepository;
    }

    public async Task TryAssignDefaultCurriculumForFirstYearAsync(
        Student student,
        string reason,
        CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(student.curriculum_code))
        {
            return;
        }

        if (!StudentYearLevelHelper.IsFirstYear(student.year_level))
        {
            return;
        }

        var program = await _programRepository.GetProgramByCodeAsync(student.program_code);
        if (program == null)
        {
            return;
        }

        var curriculum = await _curriculaRepository.GetMostRecentActiveForProgramAsync(
            program.program_id,
            cancellationToken);
        if (curriculum == null)
        {
            return;
        }

        await AssignCurriculumIfMissingAsync(
            student,
            curriculum.curriculum_code.Trim(),
            reason,
            cancellationToken);
    }

    public async Task AssignCurriculumIfMissingAsync(
        Student student,
        string curriculumCode,
        string reason,
        CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(student.curriculum_code))
        {
            return;
        }

        var code = curriculumCode.Trim();
        if (code.Length == 0)
        {
            return;
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
            throw new InvalidOperationException(
                $"Curriculum '{code}' does not belong to the student's program ({student.program_code}).");
        }

        student.SetCurriculumCode(code);
        await _studentRepository.UpdateStudentAsync(student);

        var history = StudentCurriculumHistory.Create(
            student.id,
            code,
            effectiveSchoolYear: null,
            reason,
            notes: null,
            migratedBy: null);

        await _curriculumHistoryRepository.CreateAsync(history, cancellationToken);
    }
}

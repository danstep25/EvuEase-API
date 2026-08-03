using AutoMapper;
using EvuEase.Application.Common;
using EvuEase.Application.DTOs.Program;
using EvuEase.Application.Interfaces.Persistence;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Application.Interfaces.Services;
using EvuEase.Domain.Entities;

namespace EvuEase.Application.Services;

public class ProgramService : IProgramService
{
    private readonly IProgramRepository _programRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ICurriculaRepository _curriculaRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IFacultyClassRepository _facultyClassRepository;
    private readonly IApplicationUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProgramService(
        IProgramRepository programRepository,
        ICourseRepository courseRepository,
        ICurriculaRepository curriculaRepository,
        IStudentRepository studentRepository,
        IFacultyClassRepository facultyClassRepository,
        IApplicationUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _programRepository = programRepository;
        _courseRepository = courseRepository;
        _curriculaRepository = curriculaRepository;
        _studentRepository = studentRepository;
        _facultyClassRepository = facultyClassRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResults<ProgramResponse>> GetAllPrograms(ProgramRequest programRequest)
    {
        var pagedEntities = await _programRepository.GetAllPrograms(programRequest);
        return pagedEntities.MapToDto<Program, ProgramResponse>(_mapper);
    }

    public async Task<ProgramResponse?> GetProgramByIdAsync(long id)
    {
        var program = await _programRepository.GetProgramByIdAsync(id);
        return program == null ? null : _mapper.Map<ProgramResponse>(program);
    }

    public async Task<string> CreateProgramAsync(CreateProgramRequest programRequest)
    {
        if (await _programRepository.ProgramCodeExistsAsync(programRequest.ProgramCode))
        {
            throw new InvalidOperationException($"Program with code '{programRequest.ProgramCode}' already exists.");
        }

        var program = Program.Create(
            programRequest.ProgramCode,
            programRequest.ProgramTitle,
            programRequest.ProgramCompletionYears,
            programRequest.ProgramTotalUnits,
            programRequest.ProgramStatus
        );

        var result = await _programRepository.CreateProgramAsync(program);
        return result.program_code;
    }

    public async Task<string> UpdateProgramAsync(UpdateProgramRequest programRequest)
    {
        var program = await _programRepository.GetProgramByIdAsync(programRequest.ProgramId);

        if (program == null)
        {
            throw new KeyNotFoundException($"Program with ID {programRequest.ProgramId} was not found.");
        }

        if (await _programRepository.ProgramCodeExistsAsync(programRequest.ProgramCode, programRequest.ProgramId))
        {
            throw new InvalidOperationException($"Program with code '{programRequest.ProgramCode}' already exists.");
        }

        program.Update(
            programRequest.ProgramCode,
            programRequest.ProgramTitle,
            programRequest.ProgramCompletionYears,
            programRequest.ProgramTotalUnits,
            programRequest.ProgramStatus
        );

        var result = await _programRepository.UpdateProgramAsync(program);
        return result.program_code;
    }

    public async Task DeleteProgramAsync(long id)
    {
        var program = await _programRepository.GetProgramByIdAsync(id);
        if (program == null)
        {
            throw new KeyNotFoundException($"Program with ID {id} was not found.");
        }

        var code = program.program_code.Trim();
        var studentCount = await _studentRepository.CountActiveByProgramCodeAsync(code);
        if (studentCount > 0)
        {
            throw new InvalidOperationException(
                $"Cannot delete this program: {studentCount} student(s) are still assigned to program code \"{code}\". Reassign or update those students first.");
        }

        var classCount = await _facultyClassRepository.CountActiveByProgramCodeAsync(code);
        if (classCount > 0)
        {
            throw new InvalidOperationException(
                $"Cannot delete this program: {classCount} active class section(s) use program code \"{code}\". Remove or update those sections first.");
        }

        await using var tx = await _unitOfWork.BeginTransactionAsync();
        await _courseRepository.SoftDeleteAllForProgramAsync(program.program_id);
        await _curriculaRepository.SoftDeleteAllForProgramAsync(program.program_id);
        await _programRepository.DeleteProgramAsync(program);
        await tx.CommitAsync();
    }
}

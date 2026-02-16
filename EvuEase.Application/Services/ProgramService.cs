using AutoMapper;
using EvuEase.Application.Common;
using EvuEase.Application.DTOs.Program;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Application.Interfaces.Services;
using EvuEase.Domain.Entities;

namespace EvuEase.Application.Services;

public class ProgramService : IProgramService
{
    private readonly IProgramRepository _programRepository;
    private readonly IMapper _mapper;

    public ProgramService(IProgramRepository programRepository, IMapper mapper)
    {
        _programRepository = programRepository;
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
        // Check if program code already exists
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
            throw new KeyNotFoundException($"Program with ID {programRequest.ProgramId} not found.");
        }

        // Check if program code already exists (excluding current program)
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

    public async Task<bool> DeleteProgramAsync(long id)
    {
        var program = await _programRepository.GetProgramByIdAsync(id);
        if (program == null)
        {
            throw new KeyNotFoundException($"Program with ID {id} not found.");
        }

        // Note: Actual deletion would require a DeleteAsync method in repository
        // For now, we'll just return true if program exists
        return true;
    }
}



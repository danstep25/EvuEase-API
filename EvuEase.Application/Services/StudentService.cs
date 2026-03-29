using AutoMapper;
using EvuEase.Application.Common;
using EvuEase.Application.DTOs.Student;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Application.Interfaces.Services;
using EvuEase.Domain.Entities;

namespace EvuEase.Application.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly IMapper _mapper;

    public StudentService(IStudentRepository studentRepository, IMapper mapper)
    {
        _studentRepository = studentRepository;
        _mapper = mapper;
    }

    public async Task<PagedResults<StudentResponse>> GetAllStudents(StudentRequest request)
    {
        var paged = await _studentRepository.GetAllStudents(request);
        return paged.MapToDto<Student, StudentResponse>(_mapper);
    }

    public async Task<StudentResponse?> GetStudentByIdAsync(long id)
    {
        var student = await _studentRepository.GetStudentByIdAsync(id);
        return student == null ? null : _mapper.Map<StudentResponse>(student);
    }

    public async Task<StudentResponse> CreateStudentAsync(CreateStudentRequest request)
    {
        if (await _studentRepository.StudentNumberExistsAsync(request.StudentNumber))
        {
            throw new InvalidOperationException($"Student number '{request.StudentNumber}' already exists.");
        }

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
        return _mapper.Map<StudentResponse>(result);
    }

    public async Task<StudentResponse> UpdateStudentAsync(UpdateStudentRequest request)
    {
        var student = await _studentRepository.GetStudentByIdAsync(request.Id);
        if (student == null)
        {
            throw new KeyNotFoundException($"Student with ID {request.Id} not found.");
        }

        if (await _studentRepository.StudentNumberExistsAsync(request.StudentNumber, request.Id))
        {
            throw new InvalidOperationException($"Student number '{request.StudentNumber}' already exists.");
        }

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
        return _mapper.Map<StudentResponse>(result);
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
}

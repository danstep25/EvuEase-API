using EvuEase.Application.Common;
using EvuEase.Application.DTOs.Student;
using EvuEase.Domain.Entities;

namespace EvuEase.Application.Interfaces.Repositories;

public interface IStudentRepository
{
    Task<PagedResults<Student>> GetAllStudents(StudentRequest request);
    Task<Student?> GetStudentByIdAsync(long id);
    Task<Student> CreateStudentAsync(Student student);
    Task<Student> UpdateStudentAsync(Student student);
    Task DeleteStudentAsync(Student student);
    Task<bool> StudentNumberExistsAsync(string studentNumber, long? excludeId = null);
}

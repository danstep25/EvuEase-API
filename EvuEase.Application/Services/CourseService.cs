using AutoMapper;
using EvuEase.Application.Common;
using EvuEase.Application.DTOs.Course;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Application.Interfaces.Services;
using EvuEase.Domain.Entities;

namespace EvuEase.Application.Services;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly ICurriculaRepository _curriculaRepository;
    private readonly IProgramRepository _programRepository;
    private readonly IMapper _mapper;

    public CourseService(
        ICourseRepository courseRepository,
        ICurriculaRepository curriculaRepository,
        IProgramRepository programRepository,
        IMapper mapper)
    {
        _courseRepository = courseRepository;
        _curriculaRepository = curriculaRepository;
        _programRepository = programRepository;
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
        var existingCourse = await _courseRepository.GetCourseByCodeAsync(courseRequest.CourseCode);
        if (existingCourse != null)
        {
            throw new Exception($"Course with code '{courseRequest.CourseCode}' already exists.");
        }

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

        var course = Course.Create(
            courseRequest.CourseCode,
            curriculum.id,
            courseRequest.ProgramId,
            courseRequest.CourseTitle,
            courseRequest.CourseTotalUnits,
            courseRequest.CourseYearLevel,
            courseRequest.CourseSemester,
            courseRequest.CourseComponent,
            courseRequest.Prerequisites,
            courseRequest.Description
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
        var course = await _courseRepository.GetCourseByCodeAsync(courseCode);
        if (course == null)
        {
            throw new Exception($"Course with code '{courseCode}' not found.");
        }

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

        course.Update(
            curriculum.id,
            courseRequest.ProgramId,
            courseRequest.CourseTitle,
            courseRequest.CourseTotalUnits,
            courseRequest.CourseYearLevel,
            courseRequest.CourseSemester,
            courseRequest.CourseComponent,
            courseRequest.Prerequisites,
            courseRequest.Description
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

    public async Task DeleteCourseAsync(string courseCode)
    {
        var course = await _courseRepository.GetCourseByCodeAsync(courseCode);
        if (course == null)
        {
            throw new Exception($"Course with code '{courseCode}' not found.");
        }
        await _courseRepository.DeleteCourseAsync(course);
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


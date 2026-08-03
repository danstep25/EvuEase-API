using EvuEase.Application.Common;
using EvuEase.Application.DTOs.TuitionFee;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Domain.Entities;
using EvuEase.Infrastructure.Common;
using EvuEase.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EvuEase.Infrastructure.Repositories;

public class TuitionFeeRepository : BaseRepository<TuitionFee>, ITuitionFeeRepository
{
    public TuitionFeeRepository(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        : base(dbContext, httpContextAccessor) { }

    public async Task<PagedResults<TuitionFee>> GetAllTuitionFees(TuitionFeeRequest tuitionFeeRequest)
    {
        var query = GetAll();

        if (!string.IsNullOrWhiteSpace(tuitionFeeRequest.SearchTerm))
        {
            var searchTerm = tuitionFeeRequest.SearchTerm.ToLower();
            query = query.Where(tf =>
                (tf.course_code != null && tf.course_code.ToLower().Contains(searchTerm)) ||
                (tf.course_title != null && tf.course_title.ToLower().Contains(searchTerm)) ||
                (tf.batch != null && tf.batch.ToLower().Contains(searchTerm)) ||
                (tf.semester != null && tf.semester.ToLower().Contains(searchTerm))
            );
        }

        if (!string.IsNullOrWhiteSpace(tuitionFeeRequest.SyId))
        {
            query = query.Where(tf => tf.sy_id != null && tf.sy_id.ToLower().Contains(tuitionFeeRequest.SyId.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(tuitionFeeRequest.Batch))
        {
            query = query.Where(tf => tf.batch != null && tf.batch.ToLower().Contains(tuitionFeeRequest.Batch.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(tuitionFeeRequest.Semester))
        {
            query = query.Where(tf => tf.semester != null && tf.semester.ToLower() == tuitionFeeRequest.Semester.ToLower());
        }

        if (!string.IsNullOrWhiteSpace(tuitionFeeRequest.CourseCode))
        {
            query = query.Where(tf => tf.course_code != null && tf.course_code.ToLower().Contains(tuitionFeeRequest.CourseCode.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(tuitionFeeRequest.CourseTitle))
        {
            query = query.Where(tf => tf.course_title != null && tf.course_title.ToLower().Contains(tuitionFeeRequest.CourseTitle.ToLower()));
        }

        return await query.PaginateAsync(
            tuitionFeeRequest.PageIndex,
            tuitionFeeRequest.PageSize,
            tuitionFeeRequest.SortKey,
            tuitionFeeRequest.SortDirection
        );
    }

    public async Task<TuitionFee?> GetTuitionFeeByIdAsync(long id)
    {
        return await GetAll()
            .FirstOrDefaultAsync(tf => tf.id == id);
    }

    public async Task<TuitionFee> CreateTuitionFeeAsync(TuitionFee tuitionFee)
    {
        await AddAsync(tuitionFee);
        await SaveChangesAsync();
        return tuitionFee;
    }

    public async Task<TuitionFee> UpdateTuitionFeeAsync(TuitionFee tuitionFee)
    {
        await UpdateAsync(tuitionFee);
        await SaveChangesAsync();
        return tuitionFee;
    }

    public async Task DeleteTuitionFeeAsync(TuitionFee tuitionFee)
    {
        await SoftDeleteAsync(tuitionFee);
        await SaveChangesAsync();
    }
}




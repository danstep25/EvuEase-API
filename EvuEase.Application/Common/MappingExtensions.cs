using AutoMapper;

namespace EvuEase.Application.Common;

public static class MappingExtensions
{
    /// <summary>
    /// Maps a PagedResults of entities to a PagedResults of DTOs
    /// </summary>
    /// <typeparam name="TEntity">The source entity type</typeparam>
    /// <typeparam name="TDto">The destination DTO type</typeparam>
    /// <param name="pagedResults">The paged results containing entities</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <returns>A new PagedResults containing mapped DTOs</returns>
    public static PagedResults<TDto> MapToDto<TEntity, TDto>(
        this PagedResults<TEntity> pagedResults, 
        IMapper mapper)
    {
        var mappedResults = mapper.Map<List<TDto>>(pagedResults.Result);

        return new PagedResults<TDto>(
            pagedResults.PageIndex,
            pagedResults.PageSize,
            pagedResults.TotalRecords,
            pagedResults.TotalEntries,
            mappedResults
        );
    }
}


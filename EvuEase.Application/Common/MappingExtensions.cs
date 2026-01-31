using AutoMapper;

namespace EvuEase.Application.Common;

public static class MappingExtensions
{
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





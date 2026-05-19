using Application.Common.Requests;
using Application.Common.Responses;
using Application.Features.Drugs.Queries.GetList;
using Application.Pipelines.Caching.Add;
using MediatR;
using Persistence.Dynamic;

namespace Application.Features.Drugs.Queries.GetListByDynamic;

public class GetListByDynamicDrugQuery : IRequest<GetListResponse<GetListByDynamicDrugListItemDto>>, ICacheableRequest
{
    public PageRequest? PageRequest { get; set; }
    public DynamicQuery? DynamicQuery { get; set; }


    public string CacheKey => $"{GetType().Name}_{PageRequest?.PageNumber}_{PageRequest?.PageSize}";
    public bool BypassCache { get; }
    public string? CacheGroupKey => "GetDrugsQuery";
    public TimeSpan? CacheExpiration { get; } = TimeSpan.FromDays(7);
}

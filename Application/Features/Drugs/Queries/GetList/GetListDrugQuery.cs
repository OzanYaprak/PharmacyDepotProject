using Application.Common.Requests;
using Application.Common.Responses;
using Application.Pipelines.Caching.Add;
using MediatR;

namespace Application.Features.Drugs.Queries.GetList;

public class GetListDrugQuery : IRequest<GetListResponse<GetListDrugListItemDto>>, ICacheableRequest
{
    public PageRequest? PageRequest { get; set; }


    public string CacheKey => $"{GetType().Name}_{PageRequest?.PageNumber}_{PageRequest?.PageSize}";
    public bool BypassCache => false;
    public string? CacheGroupKey => "GetDrugsQuery";
    public TimeSpan? CacheExpiration { get; } = TimeSpan.FromDays(7);
}

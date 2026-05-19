using Application.Common.Requests;
using Application.Common.Responses;
using Application.Pipelines.Caching.Add;
using MediatR;
using Persistence.Dynamic;

namespace Application.Features.Customers.Queries.GetListByDynamic;

public class GetListByDynamicCustomerQuery : IRequest<GetListResponse<GetListByDynamicCustomerListItemDto>>, ICacheableRequest
{
    public PageRequest? PageRequest { get; set; }
    public DynamicQuery? DynamicQuery { get; set; }

    public string CacheKey => $"{GetType().Name}_{PageRequest?.PageNumber}_{PageRequest?.PageSize}";
    public bool BypassCache { get; }
    public string? CacheGroupKey => "GetCustomersQuery";
    public TimeSpan? CacheExpiration { get; } = TimeSpan.FromDays(7);
}

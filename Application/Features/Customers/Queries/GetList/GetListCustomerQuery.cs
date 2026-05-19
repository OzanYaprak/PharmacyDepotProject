using Application.Common.Responses;
using MediatR;
using Application.Common.Requests;
using Application.Pipelines.Caching.Add;

namespace Application.Features.Customers.Queries.GetList;

public class GetListCustomerQuery : IRequest<GetListResponse<GetListCustomerListItemDto>>, ICacheableRequest
{
    public PageRequest? PageRequest { get; set; }


    public string CacheKey => $"{GetType().Name}_{PageRequest?.PageNumber}_{PageRequest?.PageSize}";
    public bool BypassCache { get; }
    public TimeSpan? CacheExpiration { get; } = TimeSpan.FromDays(7);
    public string? CacheGroupKey => "GetCustomersQuery";
}

using Application.Common.Responses;
using MediatR;
using Application.Common.Requests;
using Application.Pipelines.Caching.Add;
using Application.Pipelines.Logging;

namespace Application.Features.Customers.Queries.GetList;

public class GetListCustomerQuery : IRequest<GetListResponse<GetListCustomerListItemDto>>, ICacheableRequest, ILoggableRequest
{
    public PageRequest? PageRequest { get; set; }


    public string CacheKey => $"{GetType().Name}_{PageRequest?.PageNumber}_{PageRequest?.PageSize}";
    public bool BypassCache { get; }
    public TimeSpan? CacheExpiration { get; } = TimeSpan.FromDays(7);
    public string? CacheGroupKey => "GetCustomersQuery";
}

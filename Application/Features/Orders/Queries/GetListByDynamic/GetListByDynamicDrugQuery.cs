using Application.Common.Requests;
using Application.Common.Responses;
using Application.Features.Drugs.Queries.GetListByDynamic;
using MediatR;
using Persistence.Dynamic;

namespace Application.Features.Orders.Queries.GetListByDynamic;

public class GetListByDynamicOrderQuery : IRequest<GetListResponse<GetListByDynamicOrderListItemDto>>
{
    public PageRequest? PageRequest { get; set; }
    public DynamicQuery? DynamicQuery { get; set; }
}

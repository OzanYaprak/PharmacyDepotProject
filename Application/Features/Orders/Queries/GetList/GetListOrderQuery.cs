using Application.Common.Requests;
using Application.Common.Responses;
using MediatR;

namespace Application.Features.Orders.Queries.GetList;

public class GetListOrderQuery : IRequest<GetListResponse<GetListOrderListItemDto>>
{
    public PageRequest? PageRequest { get; set; }
}

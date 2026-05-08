using Application.Common.Requests;
using Application.Common.Responses;
using MediatR;

namespace Application.Features.Sales.Queries.GetList;

public class GetListSaleQuery : IRequest<GetListResponse<GetListSaleListItemDto>>
{
    public PageRequest? PageRequest { get; set; }
}

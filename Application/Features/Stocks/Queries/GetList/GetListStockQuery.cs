using Application.Common.Requests;
using Application.Common.Responses;
using MediatR;

namespace Application.Features.Stocks.Queries.GetList;

public class GetListStockQuery : IRequest<GetListResponse<GetListStockListItemDto>>
{
    public PageRequest? PageRequest { get; set; }
}

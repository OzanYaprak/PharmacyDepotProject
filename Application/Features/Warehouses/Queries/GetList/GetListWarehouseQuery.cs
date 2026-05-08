using Application.Common.Requests;
using Application.Common.Responses;
using MediatR;

namespace Application.Features.Warehouses.Queries.GetList;

public class GetListWarehouseQuery : IRequest<GetListResponse<GetListWarehouseListItemDto>>
{
    public PageRequest? PageRequest { get; set; }
}

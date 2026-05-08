using Application.Common.Requests;
using Application.Common.Responses;
using MediatR;

namespace Application.Features.Suppliers.Queries.GetList;

public class GetListSupplierQuery : IRequest<GetListResponse<GetListSupplierListItemDto>>
{
    public PageRequest? PageRequest { get; set; }
}

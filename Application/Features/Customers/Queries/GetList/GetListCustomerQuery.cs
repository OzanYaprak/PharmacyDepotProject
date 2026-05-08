using Application.Common.Responses;
using MediatR;
using Application.Common.Requests;

namespace Application.Features.Customers.Queries.GetList;

public class GetListCustomerQuery : IRequest<GetListResponse<GetListCustomerListItemDto>>
{
    public PageRequest? PageRequest { get; set; }
}

using Application.Common.Requests;
using Application.Common.Responses;
using MediatR;

namespace Application.Features.Drugs.Queries.GetList;

public class GetListDrugQuery : IRequest<GetListResponse<GetListDrugListItemDTO>>
{
    public PageRequest? PageRequest { get; set; }
}

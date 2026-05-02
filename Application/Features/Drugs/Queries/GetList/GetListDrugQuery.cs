using Application.Features.Drugs.Requests;
using Application.Features.Drugs.Responses;
using MediatR;

namespace Application.Features.Drugs.Queries.GetList;

public class GetListDrugQuery : IRequest<GetListResponse<GetListDrugListItemDTO>>
{
    public PageRequest? PageRequest { get; set; }
}

using Application.Common.Requests;
using Application.Common.Responses;
using Application.Features.Drugs.Queries.GetList;
using MediatR;
using Persistence.Dynamic;

namespace Application.Features.Drugs.Queries.GetListByDynamic;

public class GetListByDynamicDrugQuery : IRequest<GetListResponse<GetListByDynamicDrugListItemDto>>
{
    public PageRequest? PageRequest { get; set; }
    public DynamicQuery? DynamicQuery { get; set; }
}

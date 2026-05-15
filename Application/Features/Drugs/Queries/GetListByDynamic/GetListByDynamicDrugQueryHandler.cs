using Application.Common.Responses;
using Application.Features.Drugs.Queries.GetList;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Paging;
using Persistence.Repositories.Drug;

namespace Application.Features.Drugs.Queries.GetListByDynamic;

public class GetListByDynamicDrugQueryHandler : IRequestHandler<GetListByDynamicDrugQuery, GetListResponse<GetListByDynamicDrugListItemDto>>
{
    #region Constructor Injection

    private readonly IDrugRepository _drugRepository;
    private readonly IMapper _mapper;
    public GetListByDynamicDrugQueryHandler(IDrugRepository drugRepository, IMapper mapper)
    {
        _drugRepository = drugRepository;
        _mapper = mapper;
    }

    #endregion

    public async Task<GetListResponse<GetListByDynamicDrugListItemDto>> Handle(GetListByDynamicDrugQuery request, CancellationToken cancellationToken)
    {
        Paginate<Drug> drugs = await _drugRepository
            .GetListByDynamicAsync(
            dynamic: request.DynamicQuery!,
            pageNumber: request.PageRequest?.PageNumber ?? 0,
            pageSize: request.PageRequest?.PageSize ?? 10,
            withDeleted: true,
            cancellationToken: cancellationToken);

        GetListResponse<GetListByDynamicDrugListItemDto> response = _mapper.Map<GetListResponse<GetListByDynamicDrugListItemDto>>(drugs);
        return response;
    }
}

using Application.Common.Responses;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Paging;
using Persistence.Repositories.Drug;

namespace Application.Features.Drugs.Queries.GetList;

public class GetListDrugQueryHandler : IRequestHandler<GetListDrugQuery, GetListResponse<GetListDrugListItemDTO>>
{
    #region Constructor Injection

    private readonly IDrugRepository _drugRepository;
    private readonly IMapper _mapper;
    public GetListDrugQueryHandler(IDrugRepository drugRepository, IMapper mapper)
    {
        _drugRepository = drugRepository;
        _mapper = mapper;
    }

    #endregion

    public async Task<GetListResponse<GetListDrugListItemDTO>> Handle(GetListDrugQuery request, CancellationToken cancellationToken)
    {
        Paginate<Drug> drugs = await _drugRepository
            .GetListAsync(
            pageNumber: request.PageRequest?.PageNumber ?? 0,
            pageSize: request.PageRequest?.PageSize ?? 10,
            withDeleted: true,
            cancellationToken: cancellationToken);

        GetListResponse<GetListDrugListItemDTO> response = _mapper.Map<GetListResponse<GetListDrugListItemDTO>>(drugs);
        return response;
    }
}

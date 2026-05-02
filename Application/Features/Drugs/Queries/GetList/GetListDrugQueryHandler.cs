using Application.Features.Drugs.Responses;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Paging;
using Persistence.Repositories.Drug;

namespace Application.Features.Drugs.Queries.GetList;

public class GetListDrugQueryHandler : IRequestHandler<GetListDrugQuery, GetListResponse<GetListDrugListItemDTO>>
{
    private readonly IDrugRepository _drugRepository;
    private readonly IMapper _mapper;
    public GetListDrugQueryHandler(IDrugRepository drugRepository, IMapper mapper)
    {
        _drugRepository = drugRepository;
        _mapper = mapper;
    }

    public async Task<GetListResponse<GetListDrugListItemDTO>> Handle(GetListDrugQuery request, CancellationToken cancellationToken)
    {
        Paginate<Drug> drugs = await _drugRepository
            .GetListAsync(
            index: request.PageRequest?.PageIndex ?? 0,
            size: request.PageRequest?.PageSize ?? 10,
            cancellationToken: cancellationToken);

        GetListResponse<GetListDrugListItemDTO> response = _mapper.Map<GetListResponse<GetListDrugListItemDTO>>(drugs);
        return response;
    }
}

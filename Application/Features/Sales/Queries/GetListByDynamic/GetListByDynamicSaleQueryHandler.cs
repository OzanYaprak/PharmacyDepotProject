using Application.Common.Responses;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Paging;
using Persistence.Repositories.Sale;

namespace Application.Features.Sales.Queries.GetListByDynamic;

public class GetListByDynamicSaleQueryHandler : IRequestHandler<GetListByDynamicSaleQuery, GetListResponse<GetListByDynamicSaleListItemDto>>
{
    #region Constructor Injection

    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    public GetListByDynamicSaleQueryHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    #endregion

    public async Task<GetListResponse<GetListByDynamicSaleListItemDto>> Handle(GetListByDynamicSaleQuery request, CancellationToken cancellationToken)
    {
        Paginate<Sale> sales = await _saleRepository
            .GetListByDynamicAsync(
            dynamic: request.DynamicQuery!,
            pageNumber: request.PageRequest?.PageNumber ?? 0,
            pageSize: request.PageRequest?.PageSize ?? 10,
            withDeleted: true,
            cancellationToken: cancellationToken);

        GetListResponse<GetListByDynamicSaleListItemDto> response = _mapper.Map<GetListResponse<GetListByDynamicSaleListItemDto>>(sales);
        return response;
    }
}

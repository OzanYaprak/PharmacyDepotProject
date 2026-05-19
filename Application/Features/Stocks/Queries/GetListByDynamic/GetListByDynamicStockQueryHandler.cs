using Application.Common.Responses;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Paging;
using Persistence.Repositories.Stock;

namespace Application.Features.Stocks.Queries.GetListByDynamic;

public class GetListByDynamicStockQueryHandler : IRequestHandler<GetListByDynamicStockQuery, GetListResponse<GetListByDynamicStockListItemDto>>
{
    #region Constructor Injection

    private readonly IStockRepository _stockRepository;
    private readonly IMapper _mapper;
    public GetListByDynamicStockQueryHandler(IStockRepository stockRepository, IMapper mapper)
    {
        _stockRepository = stockRepository;
        _mapper = mapper;
    }

    #endregion

    public async Task<GetListResponse<GetListByDynamicStockListItemDto>> Handle(GetListByDynamicStockQuery request, CancellationToken cancellationToken)
    {
        Paginate<Stock> stocks = await _stockRepository
            .GetListByDynamicAsync(
            dynamic: request.DynamicQuery!,
            pageNumber: request.PageRequest?.PageNumber ?? 0,
            pageSize: request.PageRequest?.PageSize ?? 10,
            withDeleted: true,
            cancellationToken: cancellationToken);

        GetListResponse<GetListByDynamicStockListItemDto> response = _mapper.Map<GetListResponse<GetListByDynamicStockListItemDto>>(stocks);
        return response;
    }
}

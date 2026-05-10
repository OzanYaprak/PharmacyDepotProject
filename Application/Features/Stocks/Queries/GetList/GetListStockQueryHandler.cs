using Application.Common.Responses;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.Paging;
using Persistence.Repositories.Stock;

namespace Application.Features.Stocks.Queries.GetList;

public class GetListStockQueryHandler : IRequestHandler<GetListStockQuery, GetListResponse<GetListStockListItemDto>>
{
    private readonly IStockRepository _stockRepository;
    private readonly IMapper _mapper;

    public GetListStockQueryHandler(IStockRepository stockRepository, IMapper mapper)
    {
        _stockRepository = stockRepository;
        _mapper = mapper;
    }

    public async Task<GetListResponse<GetListStockListItemDto>> Handle(GetListStockQuery request, CancellationToken cancellationToken)
    {
        Paginate<Stock> stocks = await _stockRepository.GetListAsync(
            include: s => s.Include(s => s.Drug).Include(s => s.Warehouse),
            pageNumber: request.PageRequest?.PageNumber ?? 0,
            pageSize: request.PageRequest?.PageSize ?? 10,
            cancellationToken: cancellationToken);

        return _mapper.Map<GetListResponse<GetListStockListItemDto>>(stocks);
    }
}

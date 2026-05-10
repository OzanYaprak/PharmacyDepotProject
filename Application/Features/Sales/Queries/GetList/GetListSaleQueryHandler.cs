using Application.Common.Responses;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.Paging;
using Persistence.Repositories.Sale;

namespace Application.Features.Sales.Queries.GetList;

public class GetListSaleQueryHandler : IRequestHandler<GetListSaleQuery, GetListResponse<GetListSaleListItemDto>>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public GetListSaleQueryHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<GetListResponse<GetListSaleListItemDto>> Handle(GetListSaleQuery request, CancellationToken cancellationToken)
    {
        Paginate<Sale> sales = await _saleRepository.GetListAsync(
            include: s => s.Include(s => s.Customer),
            pageNumber: request.PageRequest?.PageNumber ?? 0,
            pageSize: request.PageRequest?.PageSize ?? 10,
            cancellationToken: cancellationToken);

        return _mapper.Map<GetListResponse<GetListSaleListItemDto>>(sales);
    }
}

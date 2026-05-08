using Application.Common.Responses;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Paging;
using Persistence.Repositories.Warehouse;

namespace Application.Features.Warehouses.Queries.GetList;

public class GetListWarehouseQueryHandler : IRequestHandler<GetListWarehouseQuery, GetListResponse<GetListWarehouseListItemDto>>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IMapper _mapper;

    public GetListWarehouseQueryHandler(IWarehouseRepository warehouseRepository, IMapper mapper)
    {
        _warehouseRepository = warehouseRepository;
        _mapper = mapper;
    }

    public async Task<GetListResponse<GetListWarehouseListItemDto>> Handle(GetListWarehouseQuery request, CancellationToken cancellationToken)
    {
        Paginate<Warehouse> warehouses = await _warehouseRepository.GetListAsync(
            pageNumber: request.PageRequest?.PageNumber ?? 0,
            pageSize: request.PageRequest?.PageSize ?? 10,
            cancellationToken: cancellationToken);

        return _mapper.Map<GetListResponse<GetListWarehouseListItemDto>>(warehouses);
    }
}

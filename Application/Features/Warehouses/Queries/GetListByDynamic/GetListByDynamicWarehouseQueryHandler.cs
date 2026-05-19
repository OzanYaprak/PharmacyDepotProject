using Application.Common.Responses;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Paging;
using Persistence.Repositories.Warehouse;

namespace Application.Features.Warehouses.Queries.GetListByDynamic;

public class GetListByDynamicWarehouseQueryHandler : IRequestHandler<GetListByDynamicWarehouseQuery, GetListResponse<GetListByDynamicWarehouseListItemDto>>
{
    #region Constructor Injection

    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IMapper _mapper;
    public GetListByDynamicWarehouseQueryHandler(IWarehouseRepository warehouseRepository, IMapper mapper)
    {
        _warehouseRepository = warehouseRepository;
        _mapper = mapper;
    }

    #endregion

    public async Task<GetListResponse<GetListByDynamicWarehouseListItemDto>> Handle(GetListByDynamicWarehouseQuery request, CancellationToken cancellationToken)
    {
        Paginate<Warehouse> warehouses = await _warehouseRepository
            .GetListByDynamicAsync(
            dynamic: request.DynamicQuery!,
            pageNumber: request.PageRequest?.PageNumber ?? 0,
            pageSize: request.PageRequest?.PageSize ?? 10,
            withDeleted: true,
            cancellationToken: cancellationToken);

        GetListResponse<GetListByDynamicWarehouseListItemDto> response = _mapper.Map<GetListResponse<GetListByDynamicWarehouseListItemDto>>(warehouses);
        return response;
    }
}

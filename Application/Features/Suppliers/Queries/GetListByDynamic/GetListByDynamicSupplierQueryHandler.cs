using Application.Common.Responses;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Paging;
using Persistence.Repositories.Supplier;

namespace Application.Features.Suppliers.Queries.GetListByDynamic;

public class GetListByDynamicSupplierQueryHandler : IRequestHandler<GetListByDynamicSupplierQuery, GetListResponse<GetListByDynamicSupplierListItemDto>>
{
    #region Constructor Injection

    private readonly ISupplierRepository _supplierRepository;
    private readonly IMapper _mapper;
    public GetListByDynamicSupplierQueryHandler(ISupplierRepository supplierRepository, IMapper mapper)
    {
        _supplierRepository = supplierRepository;
        _mapper = mapper;
    }

    #endregion

    public async Task<GetListResponse<GetListByDynamicSupplierListItemDto>> Handle(GetListByDynamicSupplierQuery request, CancellationToken cancellationToken)
    {
        Paginate<Supplier> suppliers = await _supplierRepository
            .GetListByDynamicAsync(
            dynamic: request.DynamicQuery!,
            pageNumber: request.PageRequest?.PageNumber ?? 0,
            pageSize: request.PageRequest?.PageSize ?? 10,
            withDeleted: true,
            cancellationToken: cancellationToken);

        GetListResponse<GetListByDynamicSupplierListItemDto> response = _mapper.Map<GetListResponse<GetListByDynamicSupplierListItemDto>>(suppliers);
        return response;
    }
}

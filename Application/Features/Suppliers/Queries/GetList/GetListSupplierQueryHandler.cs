using Application.Common.Responses;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Paging;
using Persistence.Repositories.Supplier;

namespace Application.Features.Suppliers.Queries.GetList;

public class GetListSupplierQueryHandler : IRequestHandler<GetListSupplierQuery, GetListResponse<GetListSupplierListItemDto>>
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly IMapper _mapper;

    public GetListSupplierQueryHandler(ISupplierRepository supplierRepository, IMapper mapper)
    {
        _supplierRepository = supplierRepository;
        _mapper = mapper;
    }

    public async Task<GetListResponse<GetListSupplierListItemDto>> Handle(GetListSupplierQuery request, CancellationToken cancellationToken)
    {
        Paginate<Supplier> suppliers = await _supplierRepository.GetListAsync(
            pageNumber: request.PageRequest?.PageNumber ?? 0,
            pageSize: request.PageRequest?.PageSize ?? 10,
            cancellationToken: cancellationToken);

        return _mapper.Map<GetListResponse<GetListSupplierListItemDto>>(suppliers);
    }
}

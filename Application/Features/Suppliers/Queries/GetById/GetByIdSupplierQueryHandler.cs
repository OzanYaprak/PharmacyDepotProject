using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Repositories.Supplier;

namespace Application.Features.Suppliers.Queries.GetById;

public class GetByIdSupplierQueryHandler : IRequestHandler<GetByIdSupplierQuery, GetByIdSupplierResponse>
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly IMapper _mapper;

    public GetByIdSupplierQueryHandler(ISupplierRepository supplierRepository, IMapper mapper)
    {
        _supplierRepository = supplierRepository;
        _mapper = mapper;
    }

    public async Task<GetByIdSupplierResponse> Handle(GetByIdSupplierQuery request, CancellationToken cancellationToken)
    {
        Supplier? supplier = await _supplierRepository.GetAsync(
            predicate: s => s.Id == request.Id,
            cancellationToken: cancellationToken);

        if (supplier is null)
            throw new KeyNotFoundException($"Supplier with id '{request.Id}' was not found.");

        return _mapper.Map<GetByIdSupplierResponse>(supplier);
    }
}

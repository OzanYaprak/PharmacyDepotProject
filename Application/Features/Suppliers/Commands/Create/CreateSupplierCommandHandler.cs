using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Repositories.Supplier;

namespace Application.Features.Suppliers.Commands.Create;

public class CreateSupplierCommandHandler : IRequestHandler<CreateSupplierCommand, CreatedSupplierResponse>
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly IMapper _mapper;

    public CreateSupplierCommandHandler(ISupplierRepository supplierRepository, IMapper mapper)
    {
        _supplierRepository = supplierRepository;
        _mapper = mapper;
    }

    public async Task<CreatedSupplierResponse> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        Supplier supplier = _mapper.Map<Supplier>(request);
        supplier.Id = Guid.NewGuid();

        var result = await _supplierRepository.AddAsync(supplier, cancellationToken);
        return _mapper.Map<CreatedSupplierResponse>(result);
    }
}

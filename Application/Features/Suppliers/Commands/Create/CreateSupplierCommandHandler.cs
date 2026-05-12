using Application.Features.Suppliers.Rules;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Repositories.Supplier;

namespace Application.Features.Suppliers.Commands.Create;

public class CreateSupplierCommandHandler : IRequestHandler<CreateSupplierCommand, CreatedSupplierResponse>
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly IMapper _mapper;
    private readonly SupplierBusinessRules _supplierBusinessRules;

    public CreateSupplierCommandHandler(ISupplierRepository supplierRepository, IMapper mapper, SupplierBusinessRules supplierBusinessRules)
    {
        _supplierRepository = supplierRepository;
        _mapper = mapper;
        _supplierBusinessRules = supplierBusinessRules;
    }

    public async Task<CreatedSupplierResponse> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        await _supplierBusinessRules.PhoneNumberCannotBeDuplicatedWhenInserted(request.Phone);
        await _supplierBusinessRules.EmailCannotBeDuplicatedWhenInserted(request.Email);

        Supplier supplier = _mapper.Map<Supplier>(request);
        supplier.Id = Guid.NewGuid();

        var result = await _supplierRepository.AddAsync(supplier, cancellationToken);
        return _mapper.Map<CreatedSupplierResponse>(result);
    }
}

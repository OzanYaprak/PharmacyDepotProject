using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Repositories.Supplier;

namespace Application.Features.Suppliers.Commands.Update;

public class UpdateSupplierCommandHandler : IRequestHandler<UpdateSupplierCommand, UpdatedSupplierResponse>
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly IMapper _mapper;

    public UpdateSupplierCommandHandler(ISupplierRepository supplierRepository, IMapper mapper)
    {
        _supplierRepository = supplierRepository;
        _mapper = mapper;
    }

    public async Task<UpdatedSupplierResponse> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
    {
        Supplier? supplier = await _supplierRepository.GetAsync(
            predicate: s => s.Id == request.Id,
            cancellationToken: cancellationToken);

        if (supplier is null)
            throw new KeyNotFoundException($"Supplier with id '{request.Id}' was not found.");

        _mapper.Map(request, supplier);

        var result = await _supplierRepository.UpdateAsync(supplier, cancellationToken);
        return _mapper.Map<UpdatedSupplierResponse>(result);
    }
}

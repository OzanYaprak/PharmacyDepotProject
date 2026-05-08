using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Repositories.Supplier;

namespace Application.Features.Suppliers.Commands.Delete;

public class DeleteSupplierCommandHandler : IRequestHandler<DeleteSupplierCommand, DeletedSupplierResponse>
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly IMapper _mapper;

    public DeleteSupplierCommandHandler(ISupplierRepository supplierRepository, IMapper mapper)
    {
        _supplierRepository = supplierRepository;
        _mapper = mapper;
    }

    public async Task<DeletedSupplierResponse> Handle(DeleteSupplierCommand request, CancellationToken cancellationToken)
    {
        Supplier? supplier = await _supplierRepository.GetAsync(
            predicate: s => s.Id == request.Id,
            cancellationToken: cancellationToken);

        if (supplier is null)
            throw new KeyNotFoundException($"Supplier with id '{request.Id}' was not found.");

        await _supplierRepository.DeleteAsync(supplier, permanent: false, cancellationToken: cancellationToken);
        return _mapper.Map<DeletedSupplierResponse>(supplier);
    }
}

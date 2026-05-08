using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Repositories.Warehouse;

namespace Application.Features.Warehouses.Commands.Delete;

public class DeleteWarehouseCommandHandler : IRequestHandler<DeleteWarehouseCommand, DeletedWarehouseResponse>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IMapper _mapper;

    public DeleteWarehouseCommandHandler(IWarehouseRepository warehouseRepository, IMapper mapper)
    {
        _warehouseRepository = warehouseRepository;
        _mapper = mapper;
    }

    public async Task<DeletedWarehouseResponse> Handle(DeleteWarehouseCommand request, CancellationToken cancellationToken)
    {
        Warehouse? warehouse = await _warehouseRepository.GetAsync(
            predicate: w => w.Id == request.Id,
            cancellationToken: cancellationToken);

        if (warehouse is null)
            throw new KeyNotFoundException($"Warehouse with id '{request.Id}' was not found.");

        await _warehouseRepository.DeleteAsync(warehouse, permanent: false, cancellationToken: cancellationToken);
        return _mapper.Map<DeletedWarehouseResponse>(warehouse);
    }
}

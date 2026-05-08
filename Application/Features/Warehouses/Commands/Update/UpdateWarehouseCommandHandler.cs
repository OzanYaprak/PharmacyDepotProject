using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Repositories.Warehouse;

namespace Application.Features.Warehouses.Commands.Update;

public class UpdateWarehouseCommandHandler : IRequestHandler<UpdateWarehouseCommand, UpdatedWarehouseResponse>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IMapper _mapper;

    public UpdateWarehouseCommandHandler(IWarehouseRepository warehouseRepository, IMapper mapper)
    {
        _warehouseRepository = warehouseRepository;
        _mapper = mapper;
    }

    public async Task<UpdatedWarehouseResponse> Handle(UpdateWarehouseCommand request, CancellationToken cancellationToken)
    {
        Warehouse? warehouse = await _warehouseRepository.GetAsync(
            predicate: w => w.Id == request.Id,
            cancellationToken: cancellationToken);

        if (warehouse is null)
            throw new KeyNotFoundException($"Warehouse with id '{request.Id}' was not found.");

        _mapper.Map(request, warehouse);

        var result = await _warehouseRepository.UpdateAsync(warehouse, cancellationToken);
        return _mapper.Map<UpdatedWarehouseResponse>(result);
    }
}

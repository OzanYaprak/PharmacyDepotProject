using Application.Features.Warehouses.Rules;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Repositories.Warehouse;

namespace Application.Features.Warehouses.Commands.Update;

public class UpdateWarehouseCommandHandler : IRequestHandler<UpdateWarehouseCommand, UpdatedWarehouseResponse>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IMapper _mapper;
    private readonly WarehouseBusinessRules _warehouseBusinessRules;

    public UpdateWarehouseCommandHandler(IWarehouseRepository warehouseRepository, IMapper mapper, WarehouseBusinessRules warehouseBusinessRules)
    {
        _warehouseRepository = warehouseRepository;
        _mapper = mapper;
        _warehouseBusinessRules = warehouseBusinessRules;
    }

    public async Task<UpdatedWarehouseResponse> Handle(UpdateWarehouseCommand request, CancellationToken cancellationToken)
    {
        if (request.Name is not null)
            await _warehouseBusinessRules.NameCannotBeDuplicatedWhenUpdated(request.Id, request.Name);

        if (request.Capacity.HasValue)
            await _warehouseBusinessRules.CapacityMustBePositive(request.Capacity.Value);

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

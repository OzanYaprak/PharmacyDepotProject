using Application.Features.Warehouses.Rules;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Repositories.Warehouse;

namespace Application.Features.Warehouses.Commands.Create;

public class CreateWarehouseCommandHandler : IRequestHandler<CreateWarehouseCommand, CreatedWarehouseResponse>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IMapper _mapper;
    private readonly WarehouseBusinessRules _warehouseBusinessRules;

    public CreateWarehouseCommandHandler(IWarehouseRepository warehouseRepository, IMapper mapper, WarehouseBusinessRules warehouseBusinessRules)
    {
        _warehouseRepository = warehouseRepository;
        _mapper = mapper;
        _warehouseBusinessRules = warehouseBusinessRules;
    }

    public async Task<CreatedWarehouseResponse> Handle(CreateWarehouseCommand request, CancellationToken cancellationToken)
    {
        await _warehouseBusinessRules.NameCannotBeDuplicatedWhenInserted(request.Name);
        await _warehouseBusinessRules.CapacityMustBePositive(request.Capacity);

        Warehouse warehouse = _mapper.Map<Warehouse>(request);
        warehouse.Id = Guid.NewGuid();

        var result = await _warehouseRepository.AddAsync(warehouse, cancellationToken);
        return _mapper.Map<CreatedWarehouseResponse>(result);
    }
}

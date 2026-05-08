using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Repositories.Warehouse;

namespace Application.Features.Warehouses.Commands.Create;

public class CreateWarehouseCommandHandler : IRequestHandler<CreateWarehouseCommand, CreatedWarehouseResponse>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IMapper _mapper;

    public CreateWarehouseCommandHandler(IWarehouseRepository warehouseRepository, IMapper mapper)
    {
        _warehouseRepository = warehouseRepository;
        _mapper = mapper;
    }

    public async Task<CreatedWarehouseResponse> Handle(CreateWarehouseCommand request, CancellationToken cancellationToken)
    {
        Warehouse warehouse = _mapper.Map<Warehouse>(request);
        warehouse.Id = Guid.NewGuid();

        var result = await _warehouseRepository.AddAsync(warehouse, cancellationToken);
        return _mapper.Map<CreatedWarehouseResponse>(result);
    }
}

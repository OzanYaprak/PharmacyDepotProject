using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Repositories.Warehouse;

namespace Application.Features.Warehouses.Queries.GetById;

public class GetByIdWarehouseQueryHandler : IRequestHandler<GetByIdWarehouseQuery, GetByIdWarehouseResponse>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IMapper _mapper;

    public GetByIdWarehouseQueryHandler(IWarehouseRepository warehouseRepository, IMapper mapper)
    {
        _warehouseRepository = warehouseRepository;
        _mapper = mapper;
    }

    public async Task<GetByIdWarehouseResponse> Handle(GetByIdWarehouseQuery request, CancellationToken cancellationToken)
    {
        Warehouse? warehouse = await _warehouseRepository.GetAsync(
            predicate: w => w.Id == request.Id,
            cancellationToken: cancellationToken);

        if (warehouse is null)
            throw new KeyNotFoundException($"Warehouse with id '{request.Id}' was not found.");

        return _mapper.Map<GetByIdWarehouseResponse>(warehouse);
    }
}

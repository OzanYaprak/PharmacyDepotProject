using MediatR;

namespace Application.Features.Warehouses.Queries.GetById;

public class GetByIdWarehouseQuery : IRequest<GetByIdWarehouseResponse>
{
    public Guid Id { get; set; }
}

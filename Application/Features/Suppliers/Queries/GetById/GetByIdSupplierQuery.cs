using MediatR;

namespace Application.Features.Suppliers.Queries.GetById;

public class GetByIdSupplierQuery : IRequest<GetByIdSupplierResponse>
{
    public Guid Id { get; set; }
}

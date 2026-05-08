namespace Application.Features.Warehouses.Commands.Delete;

public class DeletedWarehouseResponse
{
    public Guid Id { get; set; }
    public DateTime? DeletedDate { get; set; }
}

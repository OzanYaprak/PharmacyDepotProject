namespace Application.Features.Suppliers.Commands.Delete;

public class DeletedSupplierResponse
{
    public Guid Id { get; set; }
    public DateTime? DeletedDate { get; set; }
}

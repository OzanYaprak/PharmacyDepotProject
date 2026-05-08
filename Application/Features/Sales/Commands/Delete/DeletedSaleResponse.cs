namespace Application.Features.Sales.Commands.Delete;

public class DeletedSaleResponse
{
    public Guid Id { get; set; }
    public DateTime? DeletedDate { get; set; }
}

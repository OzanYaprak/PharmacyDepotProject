namespace Application.Features.Stocks.Commands.Delete;

public class DeletedStockResponse
{
    public Guid Id { get; set; }
    public DateTime? DeletedDate { get; set; }
}

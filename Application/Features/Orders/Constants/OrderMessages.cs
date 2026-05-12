namespace Application.Features.Orders.Constants;

public class OrderMessages
{
    public const string OrderDateCannotBeInTheFuture = "Order date cannot be a future date.";
    public const string CancelledOrderCannotBeUpdated = "A cancelled order cannot be updated.";
    public const string DeliveredOrderCannotBeUpdated = "A delivered order cannot be updated.";
    public const string NotFound = "Order not found.";
}

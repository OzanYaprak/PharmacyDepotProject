namespace Application.Features.Stocks.Constants;

public class StockMessages
{
    public const string StockAlreadyExistsForDrugAndWarehouse = "A stock record for this drug in the specified warehouse already exists.";
    public const string QuantityMustBeNonNegative = "Stock quantity cannot be negative.";
    public const string UnitPriceMustBePositive = "Unit price must be greater than zero.";
    public const string NotFound = "Stock not found.";
}

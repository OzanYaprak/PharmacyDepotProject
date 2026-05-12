namespace Application.Features.Drugs.Constants;

public class DrugMessages
{
    public const string GtinExists = "A drug with this GTIN already exists.";
    public const string SerialNumberExists = "A drug with this serial number (SN) already exists.";
    public const string ExpireDateCannotBeInThePast = "The expiration date cannot be in the past.";
    public const string NotFound = "Drug not found.";
}

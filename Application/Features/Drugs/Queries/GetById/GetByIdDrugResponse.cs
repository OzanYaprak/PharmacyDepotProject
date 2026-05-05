namespace Application.Features.Drugs.Queries.GetById;

public class GetByIdDrugResponse
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? GTIN { get; set; }
    public string? SN { get; set; }
    public string? BN { get; set; }
    public DateTime ExpireDate { get; set; }
}

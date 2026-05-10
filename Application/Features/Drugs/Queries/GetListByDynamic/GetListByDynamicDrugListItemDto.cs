namespace Application.Features.Drugs.Queries.GetListByDynamic;

public class GetListByDynamicDrugListItemDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? GTIN { get; set; }
    public string? SN { get; set; }
    public string? BN { get; set; }
    public DateTime ExpireDate { get; set; }
}

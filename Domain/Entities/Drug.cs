namespace Domain.Entities;

public class Drug : BaseEntity<Guid>
{
    protected Drug() { }
    public Drug(Guid id, string name, string gtin, string sn, string bn, DateTime expireDate)
    {
        Id = id;
        Name = name;
        GTIN = gtin;
        SN = sn;
        BN = bn;
        ExpireDate = expireDate.AddYears(3);
    }

    public string Name { get; set; }
    public string GTIN { get; set; }
    public string SN { get; set; }
    public string BN { get; set; }
    public DateTime ExpireDate { get; set; }
}

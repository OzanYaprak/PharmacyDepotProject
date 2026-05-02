using Domain.Entities.Base;

namespace Domain.Entities;

/// <summary>
/// İlaç bilgilerini temsil eden domain entity sınıfı.
/// </summary>
public class Drug : BaseEntity<Guid>
{
    /// <summary>
    /// EF Core için parametresiz constructor. Doğrudan kullanılmaz.
    /// </summary>
    protected Drug() { }

    /// <summary>
    /// Yeni bir ilaç nesnesi oluşturur.
    /// </summary>
    /// <param name="id">İlacın benzersiz kimliği.</param>
    /// <param name="name">İlacın adı.</param>
    /// <param name="gtin">Global Ticari Ürün Numarası (GTIN).</param>
    /// <param name="sn">Seri numarası (Serial Number).</param>
    /// <param name="bn">Parti numarası (Batch Number).</param>
    /// <param name="expireDate">İlacın son kullanma tarihi; 3 yıl eklenerek hesaplanır.</param>
    public Drug(Guid id, string name, string gtin, string sn, string bn, DateTime expireDate)
    {
        Id = id;
        Name = name;
        GTIN = gtin;
        SN = sn;
        BN = bn;
        ExpireDate = expireDate.AddYears(3);
    }

    /// <summary>İlacın adı.</summary>
    public string Name { get; set; }

    /// <summary>Global Ticari Ürün Numarası (GTIN).</summary>
    public string GTIN { get; set; }

    /// <summary>Seri numarası (Serial Number).</summary>
    public string SN { get; set; }

    /// <summary>Parti numarası (Batch Number).</summary>
    public string BN { get; set; }

    /// <summary>İlacın son kullanma tarihi.</summary>
    public DateTime ExpireDate { get; set; }
}

using Domain.Entities.Base;

namespace Domain.Entities;

/// <summary>
/// Bir satışa ait ilaç kalemini temsil eden entity sınıfı.
/// </summary>
public class SaleItem : BaseEntity<Guid>
{
    /// <summary>
    /// EF Core için parametresiz constructor.
    /// </summary>
    public SaleItem() { }

    /// <summary>
    /// Yeni bir satış kalemi nesnesi oluşturur.
    /// </summary>
    /// <param name="id">Satış kaleminin benzersiz kimliği.</param>
    /// <param name="saleId">İlgili satışın kimliği.</param>
    /// <param name="drugId">İlgili ilacın kimliği.</param>
    /// <param name="quantity">Satılan miktar.</param>
    /// <param name="unitPrice">Birim fiyat.</param>
    public SaleItem(Guid id, Guid saleId, Guid drugId, int quantity, decimal unitPrice)
    {
        Id = id;
        SaleId = saleId;
        DrugId = drugId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    /// <summary>İlgili satışın yabancı anahtar kimliği.</summary>
    public Guid SaleId { get; set; }

    /// <summary>İlgili ilacın yabancı anahtar kimliği.</summary>
    public Guid DrugId { get; set; }

    /// <summary>Satılan miktar.</summary>
    public int Quantity { get; set; }

    /// <summary>Birim fiyat.</summary>
    public decimal UnitPrice { get; set; }

    // Navigation Properties
    /// <summary>Bu kalemin ait olduğu satış bilgisi.</summary>
    public virtual Sale Sale { get; set; } = default!;

    /// <summary>Bu kalemde satılan ilaç bilgisi.</summary>
    public virtual Drug Drug { get; set; } = default!;
}

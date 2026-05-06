using Domain.Entities.Base;

namespace Domain.Entities;

/// <summary>
/// Bir siparişe ait ilaç kalemini temsil eden entity sınıfı.
/// </summary>
public class OrderItem : BaseEntity<Guid>
{
    /// <summary>
    /// EF Core için parametresiz constructor.
    /// </summary>
    public OrderItem() { }

    /// <summary>
    /// Yeni bir sipariş kalemi nesnesi oluşturur.
    /// </summary>
    /// <param name="id">Sipariş kaleminin benzersiz kimliği.</param>
    /// <param name="orderId">İlgili siparişin kimliği.</param>
    /// <param name="drugId">İlgili ilacın kimliği.</param>
    /// <param name="quantity">Sipariş edilen miktar.</param>
    /// <param name="unitPrice">Birim fiyat.</param>
    public OrderItem(Guid id, Guid orderId, Guid drugId, int quantity, decimal unitPrice)
    {
        Id = id;
        OrderId = orderId;
        DrugId = drugId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    /// <summary>İlgili siparişin yabancı anahtar kimliği.</summary>
    public Guid OrderId { get; set; }

    /// <summary>İlgili ilacın yabancı anahtar kimliği.</summary>
    public Guid DrugId { get; set; }

    /// <summary>Sipariş edilen miktar.</summary>
    public int Quantity { get; set; }

    /// <summary>Birim fiyat.</summary>
    public decimal UnitPrice { get; set; }

    // Navigation Properties
    /// <summary>Bu kalemin ait olduğu sipariş bilgisi.</summary>
    public virtual Order Order { get; set; } = default!;

    /// <summary>Bu kalemde sipariş edilen ilaç bilgisi.</summary>
    public virtual Drug Drug { get; set; } = default!;
}

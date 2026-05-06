using Domain.Entities.Base;

namespace Domain.Entities;

/// <summary>
/// İlaç stok hareketini temsil eden entity sınıfı.
/// </summary>
public class Stock : BaseEntity<Guid>
{
    /// <summary>
    /// EF Core için parametresiz constructor.
    /// </summary>
    public Stock() { }

    /// <summary>
    /// Yeni bir stok hareketi nesnesi oluşturur.
    /// </summary>
    /// <param name="id">Stok kaydının benzersiz kimliği.</param>
    /// <param name="drugId">İlgili ilacın kimliği.</param>
    /// <param name="warehouseId">İlgili deponun kimliği.</param>
    /// <param name="quantity">Stok miktarı.</param>
    /// <param name="unitPrice">Birim fiyat.</param>
    public Stock(Guid id, Guid drugId, Guid warehouseId, int quantity, decimal unitPrice)
    {
        Id = id;
        DrugId = drugId;
        WarehouseId = warehouseId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    /// <summary>İlgili ilacın yabancı anahtar kimliği.</summary>
    public Guid DrugId { get; set; }

    /// <summary>İlgili deponun yabancı anahtar kimliği.</summary>
    public Guid WarehouseId { get; set; }

    /// <summary>Stok miktarı.</summary>
    public int Quantity { get; set; }

    /// <summary>Birim fiyat.</summary>
    public decimal UnitPrice { get; set; }

    // Navigation Properties
    /// <summary>Bu stok kaydına ait ilaç bilgisi.</summary>
    public virtual Drug Drug { get; set; } = default!;

    /// <summary>Bu stok kaydına ait depo bilgisi.</summary>
    public virtual Warehouse Warehouse { get; set; } = default!;
}

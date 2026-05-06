using Domain.Entities.Base;
using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Tedarikçiye yapılan satın alma siparişini temsil eden entity sınıfı.
/// </summary>
public class Order : BaseEntity<Guid>
{
    /// <summary>
    /// EF Core için parametresiz constructor.
    /// </summary>
    public Order() { }

    /// <summary>
    /// Yeni bir sipariş nesnesi oluşturur.
    /// </summary>
    /// <param name="id">Siparişin benzersiz kimliği.</param>
    /// <param name="supplierId">İlgili tedarikçinin kimliği.</param>
    /// <param name="orderDate">Sipariş tarihi.</param>
    /// <param name="status">Siparişin durumu.</param>
    public Order(Guid id, Guid supplierId, DateTime orderDate, OrderStatus status = OrderStatus.Pending)
    {
        Id = id;
        SupplierId = supplierId;
        OrderDate = orderDate;
        Status = status;
    }

    /// <summary>İlgili tedarikçinin yabancı anahtar kimliği.</summary>
    public Guid SupplierId { get; set; }

    /// <summary>Siparişin oluşturulma tarihi.</summary>
    public DateTime OrderDate { get; set; }

    /// <summary>Siparişin güncel durumu.</summary>
    public OrderStatus Status { get; set; }

    // Navigation Properties
    /// <summary>Bu siparişe ait tedarikçi bilgisi.</summary>
    public virtual Supplier Supplier { get; set; } = default!;

    /// <summary>Bu siparişe ait sipariş kalemleri.</summary>
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

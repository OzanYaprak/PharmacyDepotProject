namespace Domain.Enums;

/// <summary>
/// Satın alma siparişinin durumunu belirten enum.
/// </summary>
public enum OrderStatus
{
    /// <summary>Sipariş oluşturuldu, onay bekliyor.</summary>
    Pending = 0,

    /// <summary>Sipariş tedarikçi tarafından onaylandı.</summary>
    Confirmed = 1,

    /// <summary>Sipariş kargoya verildi.</summary>
    Shipped = 2,

    /// <summary>Sipariş teslim edildi.</summary>
    Delivered = 3,

    /// <summary>Sipariş iptal edildi.</summary>
    Cancelled = 4
}

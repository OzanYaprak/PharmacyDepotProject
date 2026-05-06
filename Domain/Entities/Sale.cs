using Domain.Entities.Base;

namespace Domain.Entities;

/// <summary>
/// Müşteri eczaneye yapılan satış kaydını temsil eden entity sınıfı.
/// </summary>
public class Sale : BaseEntity<Guid>
{
    /// <summary>
    /// EF Core için parametresiz constructor.
    /// </summary>
    public Sale() { }

    /// <summary>
    /// Yeni bir satış kaydı nesnesi oluşturur.
    /// </summary>
    /// <param name="id">Satışın benzersiz kimliği.</param>
    /// <param name="customerId">İlgili müşterinin kimliği.</param>
    /// <param name="saleDate">Satış tarihi.</param>
    /// <param name="totalAmount">Satışın toplam tutarı.</param>
    public Sale(Guid id, Guid customerId, DateTime saleDate, decimal totalAmount)
    {
        Id = id;
        CustomerId = customerId;
        SaleDate = saleDate;
        TotalAmount = totalAmount;
    }

    /// <summary>İlgili müşterinin yabancı anahtar kimliği.</summary>
    public Guid CustomerId { get; set; }

    /// <summary>Satışın gerçekleştiği tarih.</summary>
    public DateTime SaleDate { get; set; }

    /// <summary>Satışın toplam tutarı.</summary>
    public decimal TotalAmount { get; set; }

    // Navigation Properties
    /// <summary>Bu satışa ait müşteri bilgisi.</summary>
    public virtual Customer Customer { get; set; } = default!;

    /// <summary>Bu satışa ait satış kalemleri.</summary>
    public virtual ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
}

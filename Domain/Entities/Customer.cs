using Domain.Entities.Base;

namespace Domain.Entities;

/// <summary>
/// Müşteri eczaneyi temsil eden entity sınıfı.
/// </summary>
public class Customer : BaseEntity<Guid>
{
    /// <summary>
    /// EF Core için parametresiz constructor.
    /// </summary>
    public Customer() { }

    /// <summary>
    /// Yeni bir müşteri nesnesi oluşturur.
    /// </summary>
    /// <param name="id">Müşterinin benzersiz kimliği.</param>
    /// <param name="name">Müşterinin adı.</param>
    /// <param name="licenseNumber">Eczane ruhsat numarası.</param>
    /// <param name="phone">Telefon numarası.</param>
    /// <param name="email">E-posta adresi.</param>
    /// <param name="address">Fiziksel adres.</param>
    public Customer(Guid id, string name, string licenseNumber, string phone, string email, string address)
    {
        Id = id;
        Name = name;
        LicenseNumber = licenseNumber;
        Phone = phone;
        Email = email;
        Address = address;
    }

    /// <summary>Müşteri eczanenin adı.</summary>
    public string Name { get; set; } = default!;

    /// <summary>Eczane ruhsat numarası.</summary>
    public string LicenseNumber { get; set; } = default!;

    /// <summary>Müşterinin telefon numarası.</summary>
    public string Phone { get; set; } = default!;

    /// <summary>Müşterinin e-posta adresi.</summary>
    public string Email { get; set; } = default!;

    /// <summary>Müşterinin fiziksel adresi.</summary>
    public string Address { get; set; } = default!;

    // Navigation Properties
    /// <summary>Bu müşteriye ait satış kayıtları.</summary>
    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
}

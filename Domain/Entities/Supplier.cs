using Domain.Entities.Base;

namespace Domain.Entities;

/// <summary>
/// Tedarikçi (Supplier) entity sınıfı.
/// İlaç deposunun ilaçlarını temin eden tedarikçileri temsil eder.
/// </summary>
public class Supplier : BaseEntity<Guid>
{
    /// <summary>
    /// Parametresiz constructor. Entity Framework Core tarafından kullanılır.
    /// </summary>
    public Supplier() { }

    /// <summary>
    /// Tedarikçi bilgilerini içeren constructor.
    /// </summary>
    /// <param name="id">Tedarikçinin benzersiz kimliği</param>
    /// <param name="name">Tedarikçinin adı</param>
    /// <param name="contactPerson">İletişim görevlisinin adı</param>
    /// <param name="phone">Telefon numarası</param>
    /// <param name="email">E-posta adresi</param>
    /// <param name="address">Fiziksel adres</param>
    public Supplier(Guid id, string name, string contactPerson, string phone, string email, string address)
    {
        // Tedarikçiye ait bilgileri initialize et
        Id = id;
        Name = name;
        ContactPerson = contactPerson;
        Phone = phone;
        Email = email;
        Address = address;
    }

    /// <summary>
    /// Tedarikçinin adı
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// İletişim görevlisinin adı
    /// </summary>
    public string ContactPerson { get; set; }

    /// <summary>
    /// Tedarikçinin telefon numarası
    /// </summary>
    public string Phone { get; set; }

    /// <summary>
    /// Tedarikçinin e-posta adresi
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Tedarikçinin fiziksel adres bilgisi
    /// </summary>
    public string Address { get; set; }
}

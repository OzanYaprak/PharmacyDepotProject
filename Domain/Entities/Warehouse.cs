using Domain.Entities.Base;

namespace Domain.Entities;

/// <summary>
/// Depo / raf bilgisini temsil eden entity sınıfı.
/// </summary>
public class Warehouse : BaseEntity<Guid>
{
    /// <summary>
    /// EF Core için parametresiz constructor.
    /// </summary>
    public Warehouse() { }

    /// <summary>
    /// Yeni bir depo nesnesi oluşturur.
    /// </summary>
    /// <param name="id">Deponun benzersiz kimliği.</param>
    /// <param name="name">Deponun adı.</param>
    /// <param name="location">Deponun konumu.</param>
    /// <param name="capacity">Deponun maksimum kapasitesi.</param>
    public Warehouse(Guid id, string name, string location, int capacity)
    {
        Id = id;
        Name = name;
        Location = location;
        Capacity = capacity;
    }

    /// <summary>Deponun adı.</summary>
    public string Name { get; set; } = default!;

    /// <summary>Deponun fiziksel konumu.</summary>
    public string Location { get; set; } = default!;

    /// <summary>Deponun maksimum stok kapasitesi.</summary>
    public int Capacity { get; set; }

    // Navigation Properties
    /// <summary>Bu depoya ait stok hareketleri.</summary>
    public virtual ICollection<Stock> Stocks { get; set; } = new List<Stock>();
}

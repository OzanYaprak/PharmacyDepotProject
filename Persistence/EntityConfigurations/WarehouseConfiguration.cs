using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

/// <summary>
/// <see cref="Warehouse"/> entity'si için EF Core tablo ve sütun konfigürasyonlarını tanımlar.
/// </summary>
public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    /// <summary>
    /// <see cref="Warehouse"/> entity'sinin veritabanı eşlemesini yapılandırır.
    /// </summary>
    /// <param name="builder">Entity özelliklerini yapılandırmak için kullanılan builder nesnesi.</param>
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        // "Warehouses" tablosu olarak eşlenir; birincil anahtar Id sütunudur.
        builder.ToTable("Warehouses").HasKey(x => x.Id);

        // Id: Her deponun benzersiz kimliği; zorunludur.
        builder.Property(x => x.Id).HasColumnName("Id").IsRequired();

        // Name: Deponun adı; maksimum 250 karakter, zorunludur.
        builder.Property(x => x.Name).HasColumnName("Name").HasMaxLength(250).IsRequired();

        // Location: Deponun fiziksel konumu (adres, bina, kat vb.); maksimum 500 karakter, zorunludur.
        builder.Property(x => x.Location).HasColumnName("Location").HasMaxLength(500).IsRequired();

        // Capacity: Depoda aynı anda tutulabilecek maksimum ilaç/ürün adedi; zorunludur.
        builder.Property(x => x.Capacity).HasColumnName("Capacity").IsRequired();

        // CreatedDate: Kaydın sisteme ilk eklendiği tarih; zorunludur.
        builder.Property(x => x.CreatedDate).HasColumnName("CreatedDate").IsRequired();

        // UpdatedDate: Kaydın en son güncellendiği tarih; güncelleme yapılmamışsa boş kalır.
        builder.Property(x => x.UpdatedDate).HasColumnName("UpdatedDate");

        // DeletedDate: Soft-delete tarihi; bu alan doluysa kayıt silinmiş olarak kabul edilir.
        builder.Property(x => x.DeletedDate).HasColumnName("DeletedDate");

        // Name sütununda benzersiz indeks: aynı adla iki farklı depo oluşturulamamasını engeller.
        builder.HasIndex(x => x.Name, name: "UK_Warehouses_Name").IsUnique();

        // Bir depo birden fazla stok kaydına sahip olabilir; WarehouseId üzerinden 1-N ilişki kurulur.
        builder.HasMany(x => x.Stocks).WithOne(s => s.Warehouse).HasForeignKey(s => s.WarehouseId);

        // Soft-delete filtresi: DeletedDate dolu olan kayıtlar tüm sorgulardan otomatik olarak dışlanır.
        builder.HasQueryFilter(q => !q.DeletedDate.HasValue);
    }
}

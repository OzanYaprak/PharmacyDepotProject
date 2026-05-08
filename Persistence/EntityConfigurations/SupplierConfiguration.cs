using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

/// <summary>
/// <see cref="Supplier"/> entity'si için EF Core tablo ve sütun konfigürasyonlarını tanımlar.
/// </summary>
public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    /// <summary>
    /// <see cref="Supplier"/> entity'sinin veritabanı eşlemesini yapılandırır.
    /// </summary>
    /// <param name="builder">Entity özelliklerini yapılandırmak için kullanılan builder nesnesi.</param>
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        // "Suppliers" tablosu olarak eşlenir; birincil anahtar Id sütunudur.
        builder.ToTable("Suppliers").HasKey(x => x.Id);

        // Id: Her tedarikçinin benzersiz kimliği; zorunludur.
        builder.Property(x => x.Id).HasColumnName("Id").IsRequired();

        // Name: Tedarikçi firmasının ticari adı; maksimum 250 karakter, zorunludur.
        builder.Property(x => x.Name).HasColumnName("Name").HasMaxLength(250).IsRequired();

        // ContactPerson: Tedarikçi ile iletişime geçilecek kişinin adı; maksimum 250 karakter, zorunludur.
        builder.Property(x => x.ContactPerson).HasColumnName("ContactPerson").HasMaxLength(250).IsRequired();

        // Phone: Tedarikçinin telefon numarası; maksimum 20 karakter, zorunludur.
        builder.Property(x => x.Phone).HasColumnName("Phone").HasMaxLength(20).IsRequired();

        // Email: Tedarikçinin e-posta adresi; benzersiz indeksle korunur; maksimum 250 karakter, zorunludur.
        builder.Property(x => x.Email).HasColumnName("Email").HasMaxLength(250).IsRequired();

        // Address: Tedarikçinin fiziksel adres bilgisi; maksimum 500 karakter, zorunludur.
        builder.Property(x => x.Address).HasColumnName("Address").HasMaxLength(500).IsRequired();

        // CreatedDate: Kaydın sisteme ilk eklendiği tarih; zorunludur.
        builder.Property(x => x.CreatedDate).HasColumnName("CreatedDate").IsRequired();

        // UpdatedDate: Kaydın en son güncellendiği tarih; güncelleme yapılmamışsa boş kalır.
        builder.Property(x => x.UpdatedDate).HasColumnName("UpdatedDate");

        // DeletedDate: Soft-delete tarihi; bu alan doluysa kayıt silinmiş olarak kabul edilir.
        builder.Property(x => x.DeletedDate).HasColumnName("DeletedDate");

        // Email sütununda benzersiz indeks: aynı e-posta adresiyle iki tedarikçi kaydı oluşturulamamasını engeller.
        builder.HasIndex(x => x.Email, name: "UK_Suppliers_Email").IsUnique();

        // Bir tedarikçi birden fazla sipariş verebilir; SupplierId üzerinden 1-N ilişki kurulur.
        builder.HasMany(x => x.Orders).WithOne(o => o.Supplier).HasForeignKey(o => o.SupplierId);

        // Soft-delete filtresi: DeletedDate dolu olan kayıtlar tüm sorgulardan otomatik olarak dışlanır.
        builder.HasQueryFilter(q => !q.DeletedDate.HasValue);
    }
}

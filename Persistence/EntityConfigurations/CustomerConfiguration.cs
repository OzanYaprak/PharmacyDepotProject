using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

/// <summary>
/// <see cref="Customer"/> entity'si için EF Core tablo ve sütun konfigürasyonlarını tanımlar.
/// </summary>
public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    /// <summary>
    /// <see cref="Customer"/> entity'sinin veritabanı eşlemesini yapılandırır.
    /// </summary>
    /// <param name="builder">Entity özelliklerini yapılandırmak için kullanılan builder nesnesi.</param>
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        // "Customers" tablosu olarak eşlenir; birincil anahtar Id sütunudur.
        builder.ToTable("Customers").HasKey(x => x.Id);

        // Id: Her müşterinin benzersiz kimliği; zorunludur.
        builder.Property(x => x.Id).HasColumnName("Id").IsRequired();

        // Name: Müşteri eczanenin ticari adı; maksimum 250 karakter, zorunludur.
        builder.Property(x => x.Name).HasColumnName("Name").HasMaxLength(250).IsRequired();

        // LicenseNumber: Sağlık Bakanlığı tarafından verilen eczane ruhsat numarası;
        // her eczane için tekil olması gerektiğinden benzersiz indeksle korunur; maksimum 50 karakter, zorunludur.
        builder.Property(x => x.LicenseNumber).HasColumnName("LicenseNumber").HasMaxLength(50).IsRequired();

        // Phone: Müşterinin iletişim telefon numarası; benzersiz indeksle korunur; maksimum 20 karakter, zorunludur.
        builder.Property(x => x.Phone).HasColumnName("Phone").HasMaxLength(20).IsRequired();

        // Email: Müşterinin e-posta adresi; fatura ve bildirim için kullanılır;
        // benzersiz indeksle korunur; maksimum 250 karakter, zorunludur.
        builder.Property(x => x.Email).HasColumnName("Email").HasMaxLength(250).IsRequired();

        // Address: Müşteri eczanenin fiziksel adres bilgisi; teslimat için kullanılır;
        // maksimum 500 karakter, zorunludur.
        builder.Property(x => x.Address).HasColumnName("Address").HasMaxLength(500).IsRequired();

        // CreatedDate: Kaydın sisteme ilk eklendiği tarih; zorunludur.
        builder.Property(x => x.CreatedDate).HasColumnName("CreatedDate").IsRequired();

        // UpdatedDate: Kaydın en son güncellendiği tarih; güncelleme yapılmamışsa boş kalır.
        builder.Property(x => x.UpdatedDate).HasColumnName("UpdatedDate");

        // DeletedDate: Soft-delete tarihi; bu alan doluysa kayıt silinmiş olarak kabul edilir.
        builder.Property(x => x.DeletedDate).HasColumnName("DeletedDate");

        // LicenseNumber sütununda benzersiz indeks: aynı ruhsat numarasıyla iki farklı müşteri
        // kaydı oluşturulamamasını engeller.
        builder.HasIndex(x => x.LicenseNumber, name: "UK_Customers_LicenseNumber").IsUnique();

        // Email sütununda benzersiz indeks: aynı e-posta adresiyle iki farklı müşteri
        // kaydı oluşturulamamasını engeller.
        builder.HasIndex(x => x.Email, name: "UK_Customers_Email").IsUnique();

        // Phone sütununda benzersiz indeks: aynı telefon numarasıyla iki farklı müşteri
        // kaydı oluşturulamamasını engeller.
        builder.HasIndex(x => x.Phone, name: "UK_Customers_Phone").IsUnique();

        // Bir müşteri birden fazla satış işlemine sahip olabilir; CustomerId üzerinden 1-N ilişki kurulur.
        builder.HasMany(x => x.Sales).WithOne(s => s.Customer).HasForeignKey(s => s.CustomerId);

        // Soft-delete filtresi: DeletedDate dolu olan kayıtlar tüm sorgulardan otomatik olarak dışlanır.
        builder.HasQueryFilter(q => !q.DeletedDate.HasValue);
    }
}

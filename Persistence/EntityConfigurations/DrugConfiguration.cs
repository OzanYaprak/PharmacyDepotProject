using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

/// <summary>
/// <see cref="Drug"/> entity'si için EF Core tablo ve sütun konfigürasyonlarını tanımlar.
/// </summary>
public class DrugConfiguration : IEntityTypeConfiguration<Drug>
{
    /// <summary>
    /// <see cref="Drug"/> entity'sinin veritabanı eşlemesini yapılandırır.
    /// </summary>
    /// <param name="builder">Entity özelliklerini yapılandırmak için kullanılan builder nesnesi.</param>
    public void Configure(EntityTypeBuilder<Drug> builder)
    {
        // "Drugs" tablosu olarak eşlenir; birincil anahtar Id sütunudur.
        builder.ToTable("Drugs").HasKey(x => x.Id);

        // Id: Her ilaç kaydının benzersiz kimliği; zorunludur.
        builder.Property(x => x.Id).HasColumnName("Id").IsRequired();

        // Name: İlacın ticari adı; maksimum 250 karakter, zorunludur.
        builder.Property(x => x.Name).HasColumnName("Name").HasMaxLength(250).IsRequired();

        // GTIN: GS1 standardına uygun Global Trade Item Number; maksimum 14 karakter, zorunludur.
        builder.Property(x => x.GTIN).HasColumnName("GTIN").HasMaxLength(14).IsRequired();

        // SN: Seri Numarası (Serial Number); üretim partisi içinde bireysel birimi tanımlar; maksimum 100 karakter, zorunludur.
        builder.Property(x => x.SN).HasColumnName("SN").HasMaxLength(100).IsRequired();

        // BN: Parti Numarası (Batch Number); aynı üretim partisindeki tüm birimleri tanımlar; maksimum 100 karakter, zorunludur.
        builder.Property(x => x.BN).HasColumnName("BN").HasMaxLength(100).IsRequired();

        // ExpireDate: İlacın son kullanma tarihi; bu tarihten sonra satış/dağıtım yapılamaz; zorunludur.
        builder.Property(x => x.ExpireDate).HasColumnName("ExpireDate").IsRequired();

        // CreatedDate: Kaydın sisteme ilk eklendiği tarih; zorunludur.
        builder.Property(x => x.CreatedDate).HasColumnName("CreatedDate").IsRequired();

        // UpdatedDate: Kaydın en son güncellendiği tarih; güncelleme yapılmamışsa boş kalır.
        builder.Property(x => x.UpdatedDate).HasColumnName("UpdatedDate");

        // DeletedDate: Soft-delete tarihi; bu alan doluysa kayıt silinmiş olarak kabul edilir.
        builder.Property(x => x.DeletedDate).HasColumnName("DeletedDate");

        // Name sütununda benzersiz indeks: aynı ticari adla iki farklı ilaç kaydı oluşturulamasını engeller.
        builder.HasIndex(indexExpression: x => x.Name, name: "UK_Drugs_Name").IsUnique();

        // GTIN sütununda benzersiz indeks: her ilaç paketinin küresel olarak eşsiz olmasını garantiler.
        builder.HasIndex(indexExpression: x => x.GTIN, name: "UK_Drugs_GTIN").IsUnique();

        // Bir ilaç, birden fazla stok kaydına sahip olabilir; DrugId üzerinden 1-N ilişki kurulur.
        builder.HasMany(x => x.Stocks).WithOne(s => s.Drug).HasForeignKey(s => s.DrugId);

        // Bir ilaç, birden fazla sipariş kalemi (OrderItem) içerebilir; DrugId üzerinden 1-N ilişki kurulur.
        builder.HasMany(x => x.OrderItems).WithOne(s => s.Drug).HasForeignKey(s => s.DrugId);

        // Bir ilaç, birden fazla satış kalemi (SaleItem) içerebilir; DrugId üzerinden 1-N ilişki kurulur.
        builder.HasMany(x => x.SaleItems).WithOne(s => s.Drug).HasForeignKey(s => s.DrugId);

        // Soft-delete filtresi: DeletedDate dolu olan kayıtlar tüm sorgulardan otomatik olarak dışlanır.
        builder.HasQueryFilter(q => !q.DeletedDate.HasValue);
    }
}

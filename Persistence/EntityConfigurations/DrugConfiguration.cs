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
        builder.ToTable("Drugs").HasKey(x => x.Id);

        /// <summary>Birincil anahtar; zorunludur.</summary>
        builder.Property(x => x.Id).HasColumnName("Id").IsRequired();

        /// <summary>İlacın ticari adı; maksimum 250 karakter, zorunludur.</summary>
        builder.Property(x => x.Name).HasColumnName("Name").HasMaxLength(250).IsRequired();

        /// <summary>GS1 standardına uygun Global Trade Item Number; maksimum 14 karakter, zorunludur.</summary>
        builder.Property(x => x.GTIN).HasColumnName("GTIN").HasMaxLength(14).IsRequired();

        /// <summary>Seri numarası (Serial Number); maksimum 100 karakter, zorunludur.</summary>
        builder.Property(x => x.SN).HasColumnName("SN").HasMaxLength(100).IsRequired();

        /// <summary>Parti numarası (Batch Number); maksimum 100 karakter, zorunludur.</summary>
        builder.Property(x => x.BN).HasColumnName("BN").HasMaxLength(100).IsRequired();

        /// <summary>Son kullanma tarihi; zorunludur.</summary>
        builder.Property(x => x.ExpireDate).HasColumnName("ExpireDate").IsRequired();

        builder.Property(x => x.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(x => x.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(x => x.DeletedDate).HasColumnName("DeletedDate");

        builder.HasQueryFilter(q => !q.DeletedDate.HasValue);
    }
}

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Persistence.EntityConfigurations;

public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    /// <summary>
    /// <see cref="SaleItem"/> entity'sinin veritabanı eşlemesini yapılandırır.
    /// </summary>
    /// <param name="builder">Entity özelliklerini yapılandırmak için kullanılan builder nesnesi.</param>
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        // "SaleItems" tablosu olarak eşlenir; birincil anahtar Id sütunudur.
        builder.ToTable("SaleItems").HasKey(x => x.Id);

        // Id: Her satış kaleminin benzersiz kimliği; zorunludur.
        builder.Property(x => x.Id).HasColumnName("Id").IsRequired();

        // SaleId: Bu kalemin hangi satışa ait olduğunu belirten yabancı anahtar; zorunludur.
        builder.Property(x => x.SaleId).HasColumnName("SaleId").IsRequired();

        // DrugId: Satılan ilacı belirten yabancı anahtar; zorunludur.
        builder.Property(x => x.DrugId).HasColumnName("DrugId").IsRequired();

        // Quantity: Satılan ilaç adedi; boş bırakılabilir.
        builder.Property(x => x.Quantity).HasColumnName("Quantity");

        // UnitPrice: Satış anındaki birim fiyat; iade/fiyat farkı hesaplamalarında kullanılır; boş bırakılabilir.
        builder.Property(x => x.UnitPrice).HasColumnName("UnitPrice");

        // CreatedDate: Kaydın sisteme ilk eklendiği tarih; zorunludur.
        builder.Property(x => x.CreatedDate).HasColumnName("CreatedDate").IsRequired();

        // UpdatedDate: Kaydın en son güncellendiği tarih; güncelleme yapılmamışsa boş kalır.
        builder.Property(x => x.UpdatedDate).HasColumnName("UpdatedDate");

        // DeletedDate: Soft-delete tarihi; bu alan doluysa kayıt silinmiş olarak kabul edilir.
        builder.Property(x => x.DeletedDate).HasColumnName("DeletedDate");

        // Her satış kalemi bir satışa (Sale) aittir; SaleId üzerinden N-1 ilişki kurulur.
        builder.HasOne(x => x.Sale).WithMany(s => s.SaleItems).HasForeignKey(x => x.SaleId);

        // Her satış kalemi bir ilaca (Drug) karşılık gelir; DrugId üzerinden N-1 ilişki kurulur.
        builder.HasOne(x => x.Drug).WithMany(d => d.SaleItems).HasForeignKey(x => x.DrugId);

        // Soft-delete filtresi: DeletedDate dolu olan kayıtlar tüm sorgulardan otomatik olarak dışlanır.
        builder.HasQueryFilter(q => !q.DeletedDate.HasValue);
    }
}
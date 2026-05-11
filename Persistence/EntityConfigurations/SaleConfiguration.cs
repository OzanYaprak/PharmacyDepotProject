using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Persistence.EntityConfigurations;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    /// <summary>
    /// <see cref="Sale"/> entity'sinin veritabanı eşlemesini yapılandırır.
    /// </summary>
    /// <param name="builder">Entity özelliklerini yapılandırmak için kullanılan builder nesnesi.</param>

    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.HasData(
            new Sale
            {
                Id = Guid.Parse("00000022-0000-0000-0000-000000000001"),
                CustomerId = Guid.Parse("cccccccc-cccc-cccc-cccc-000000000001"),
                SaleDate = new DateTime(2025, 3, 1, 0, 0, 0, DateTimeKind.Utc),
                TotalAmount = 1250.00m,
                CreatedDate = new DateTime(2025, 3, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Sale
            {
                Id = Guid.Parse("00000022-0000-0000-0000-000000000002"),
                CustomerId = Guid.Parse("cccccccc-cccc-cccc-cccc-000000000002"),
                SaleDate = new DateTime(2025, 3, 5, 0, 0, 0, DateTimeKind.Utc),
                TotalAmount = 875.00m,
                CreatedDate = new DateTime(2025, 3, 5, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        // "Sales" tablosu olarak eşlenir; birincil anahtar Id sütunudur.
        builder.ToTable("Sales").HasKey(x => x.Id);

        // Id: Her satış işleminin benzersiz kimliği; zorunludur.
        builder.Property(x => x.Id).HasColumnName("Id").IsRequired();

        // CustomerId: Satışın hangi müşteriye yapıldığını belirten yabancı anahtar; zorunludur.
        builder.Property(x => x.CustomerId).HasColumnName("CustomerId").IsRequired();

        // SaleDate: Satışın gerçekleştiği tarih; zorunludur.
        builder.Property(x => x.SaleDate).HasColumnName("SaleDate").IsRequired();

        // TotalAmount: Satışın toplam tutarı; tüm satış kalemlerinin toplamını ifade eder; zorunludur.
        builder.Property(x => x.TotalAmount).HasColumnName("TotalAmount").IsRequired();

        // CreatedDate: Kaydın sisteme ilk eklendiği tarih; zorunludur.
        builder.Property(x => x.CreatedDate).HasColumnName("CreatedDate").IsRequired();

        // UpdatedDate: Kaydın en son güncellendiği tarih; güncelleme yapılmamışsa boş kalır.
        builder.Property(x => x.UpdatedDate).HasColumnName("UpdatedDate");

        // DeletedDate: Soft-delete tarihi; bu alan doluysa kayıt silinmiş olarak kabul edilir.
        builder.Property(x => x.DeletedDate).HasColumnName("DeletedDate");

        // Her satış bir müşteriye (Customer) aittir; CustomerId üzerinden N-1 ilişki kurulur.
        builder.HasOne(x => x.Customer).WithMany(c => c.Sales).HasForeignKey(x => x.CustomerId);

        // Bir satış birden fazla satış kalemi (SaleItem) içerebilir; SaleId üzerinden 1-N ilişki kurulur.
        builder.HasMany(x => x.SaleItems).WithOne(si => si.Sale).HasForeignKey(si => si.SaleId);

        // Soft-delete filtresi: DeletedDate dolu olan kayıtlar tüm sorgulardan otomatik olarak dışlanır.
        builder.HasQueryFilter(q => !q.DeletedDate.HasValue);
    }
}
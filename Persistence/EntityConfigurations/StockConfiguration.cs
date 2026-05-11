using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Persistence.EntityConfigurations;

public class StockConfiguration : IEntityTypeConfiguration<Stock>
{
    /// <summary>
    /// <see cref="Stock"/> entity'sinin veritabanı eşlemesini yapılandırır.
    /// </summary>
    /// <param name="builder">Entity özelliklerini yapılandırmak için kullanılan builder nesnesi.</param>
    public void Configure(EntityTypeBuilder<Stock> builder)
    {

        builder.HasData(
            new Stock
            {
                Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-000000000001"),
                DrugId = Guid.Parse("dddddddd-dddd-dddd-dddd-000000000001"),
                WarehouseId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-000000000001"),
                Quantity = 5000,
                UnitPrice = 12.50m,
                CreatedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Stock
            {
                Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-000000000002"),
                DrugId = Guid.Parse("dddddddd-dddd-dddd-dddd-000000000002"),
                WarehouseId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-000000000001"),
                Quantity = 3000,
                UnitPrice = 45.00m,
                CreatedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Stock
            {
                Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-000000000003"),
                DrugId = Guid.Parse("dddddddd-dddd-dddd-dddd-000000000003"),
                WarehouseId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-000000000002"),
                Quantity = 2000,
                UnitPrice = 8.75m,
                CreatedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        // "Stocks" tablosu olarak eşlenir; birincil anahtar Id sütunudur.
        builder.ToTable("Stocks").HasKey(x => x.Id);

        // Id: Her stok kaydının benzersiz kimliği; zorunludur.
        builder.Property(x => x.Id).HasColumnName("Id").IsRequired();

        // DrugId: Stok kaydının hangi ilaca ait olduğunu belirten yabancı anahtar; zorunludur.
        builder.Property(x => x.DrugId).HasColumnName("DrugId").IsRequired();

        // WarehouseId: Stok kaydının hangi depoda tutulduğunu belirten yabancı anahtar; zorunludur.
        builder.Property(x => x.WarehouseId).HasColumnName("WarehouseId").IsRequired();

        // Quantity: Depodaki mevcut ilaç miktarı; boş bırakılabilir.
        builder.Property(x => x.Quantity).HasColumnName("Quantity");

        // UnitPrice: İlacın birim satış/alış fiyatı; boş bırakılabilir.
        builder.Property(x => x.UnitPrice).HasColumnName("UnitPrice");

        // CreatedDate: Kaydın sisteme ilk eklendiği tarih; zorunludur.
        builder.Property(x => x.CreatedDate).HasColumnName("CreatedDate").IsRequired();

        // UpdatedDate: Kaydın en son güncellendiği tarih; güncelleme yapılmamışsa boş kalır.
        builder.Property(x => x.UpdatedDate).HasColumnName("UpdatedDate");

        // DeletedDate: Soft-delete tarihi; bu alan doluysa kayıt silinmiş olarak kabul edilir.
        builder.Property(x => x.DeletedDate).HasColumnName("DeletedDate");

        // Her stok kaydı bir ilaca (Drug) aittir; DrugId üzerinden N-1 ilişki kurulur.
        builder.HasOne(x => x.Drug).WithMany(s => s.Stocks).HasForeignKey(x => x.DrugId);

        // Her stok kaydı bir depoya (Warehouse) aittir; WarehouseId üzerinden N-1 ilişki kurulur.
        builder.HasOne(x => x.Warehouse).WithMany(d => d.Stocks).HasForeignKey(x => x.WarehouseId);

        // Soft-delete filtresi: DeletedDate dolu olan kayıtlar tüm sorgulardan otomatik olarak dışlanır.
        builder.HasQueryFilter(q => !q.DeletedDate.HasValue);
    }
}

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Persistence.EntityConfigurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    /// <summary>
    /// <see cref="Order"/> entity'sinin veritabanı eşlemesini yapılandırır.
    /// </summary>
    /// <param name="builder">Entity özelliklerini yapılandırmak için kullanılan builder nesnesi.</param>
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasData(
            new OrderItem
            {
                Id = Guid.Parse("00000011-0000-0000-0000-000000000001"),
                OrderId = Guid.Parse("ffff0001-0000-0000-0000-000000000001"),
                DrugId = Guid.Parse("dddddddd-dddd-dddd-dddd-000000000001"),
                Quantity = 1000,
                UnitPrice = 10.00m,
                CreatedDate = new DateTime(2025, 1, 5, 0, 0, 0, DateTimeKind.Utc)
            },
            new OrderItem
            {
                Id = Guid.Parse("00000011-0000-0000-0000-000000000002"),
                OrderId = Guid.Parse("ffff0001-0000-0000-0000-000000000001"),
                DrugId = Guid.Parse("dddddddd-dddd-dddd-dddd-000000000002"),
                Quantity = 500,
                UnitPrice = 40.00m,
                CreatedDate = new DateTime(2025, 1, 5, 0, 0, 0, DateTimeKind.Utc)
            },
            new OrderItem
            {
                Id = Guid.Parse("00000011-0000-0000-0000-000000000003"),
                OrderId = Guid.Parse("ffff0001-0000-0000-0000-000000000002"),
                DrugId = Guid.Parse("dddddddd-dddd-dddd-dddd-000000000003"),
                Quantity = 800,
                UnitPrice = 7.50m,
                CreatedDate = new DateTime(2025, 2, 10, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        // "OrderItems" tablosu olarak eşlenir; birincil anahtar Id sütunudur.
        builder.ToTable("OrderItems").HasKey(x => x.Id);

        // Id: Her sipariş kaleminin benzersiz kimliği; zorunludur.
        builder.Property(x => x.Id).HasColumnName("Id").IsRequired();

        // OrderId: Bu kalem hangi siparişe ait olduğunu belirten yabancı anahtar; zorunludur.
        builder.Property(x => x.OrderId).HasColumnName("OrderId").IsRequired();

        // DrugId: Bu kalemde sipariş edilen ilacı belirten yabancı anahtar; zorunludur.
        builder.Property(x => x.DrugId).HasColumnName("DrugId").IsRequired();

        // Quantity: Sipariş edilen ilaç adedi; boş bırakılabilir.
        builder.Property(x => x.Quantity).HasColumnName("Quantity");

        // UnitPrice: Sipariş anındaki birim fiyat; boş bırakılabilir.
        builder.Property(x => x.UnitPrice).HasColumnName("UnitPrice");

        // CreatedDate: Kaydın sisteme ilk eklendiği tarih; zorunludur.
        builder.Property(x => x.CreatedDate).HasColumnName("CreatedDate").IsRequired();

        // UpdatedDate: Kaydın en son güncellendiği tarih; güncelleme yapılmamışsa boş kalır.
        builder.Property(x => x.UpdatedDate).HasColumnName("UpdatedDate");

        // DeletedDate: Soft-delete tarihi; bu alan doluysa kayıt silinmiş olarak kabul edilir.
        builder.Property(x => x.DeletedDate).HasColumnName("DeletedDate");

        // Her sipariş kalemi bir siparişe (Order) aittir; OrderId üzerinden N-1 ilişki kurulur.
        builder.HasOne(x => x.Order).WithMany(o => o.OrderItems).HasForeignKey(x => x.OrderId);

        // Her sipariş kalemi bir ilaca (Drug) karşılık gelir; DrugId üzerinden N-1 ilişki kurulur.
        builder.HasOne(x => x.Drug).WithMany(d => d.OrderItems).HasForeignKey(x => x.DrugId);

        // Soft-delete filtresi: DeletedDate dolu olan kayıtlar tüm sorgulardan otomatik olarak dışlanır.
        builder.HasQueryFilter(q => !q.DeletedDate.HasValue);
    }
}
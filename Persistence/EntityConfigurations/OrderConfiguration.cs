using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Persistence.EntityConfigurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    /// <summary>
    /// <see cref="Order"/> entity'sinin veritabanı eşlemesini yapılandırır.
    /// </summary>
    /// <param name="builder">Entity özelliklerini yapılandırmak için kullanılan builder nesnesi.</param>
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        // "Orders" tablosu olarak eşlenir; birincil anahtar Id sütunudur.
        builder.ToTable("Orders").HasKey(x => x.Id);

        // Id: Her siparişin benzersiz kimliği; zorunludur.
        builder.Property(x => x.Id).HasColumnName("Id").IsRequired();

        // SupplierId: Siparişin hangi tedarikçiye verildiğini belirten yabancı anahtar; zorunludur.
        builder.Property(x => x.SupplierId).HasColumnName("SupplierId").IsRequired();

        // OrderDate: Siparişin verildiği tarih; zorunludur.
        builder.Property(x => x.OrderDate).HasColumnName("OrderDate").IsRequired();

        // Status: Siparişin mevcut durumu (beklemede, onaylandı, tamamlandı vb.); zorunludur.
        builder.Property(x => x.Status).HasColumnName("Status").IsRequired();

        // CreatedDate: Kaydın sisteme ilk eklendiği tarih; zorunludur.
        builder.Property(x => x.CreatedDate).HasColumnName("CreatedDate").IsRequired();

        // UpdatedDate: Kaydın en son güncellendiği tarih; güncelleme yapılmamışsa boş kalır.
        builder.Property(x => x.UpdatedDate).HasColumnName("UpdatedDate");

        // DeletedDate: Soft-delete tarihi; bu alan doluysa kayıt silinmiş olarak kabul edilir.
        builder.Property(x => x.DeletedDate).HasColumnName("DeletedDate");

        // Her sipariş bir tedarikçiye (Supplier) aittir; SupplierId üzerinden N-1 ilişki kurulur.
        builder.HasOne(x => x.Supplier).WithMany(s => s.Orders).HasForeignKey(x => x.SupplierId);

        // Bir sipariş birden fazla sipariş kalemi (OrderItem) içerebilir; OrderId üzerinden 1-N ilişki kurulur.
        builder.HasMany(x => x.OrderItems).WithOne(oi => oi.Order).HasForeignKey(oi => oi.OrderId);

        // Soft-delete filtresi: DeletedDate dolu olan kayıtlar tüm sorgulardan otomatik olarak dışlanır.
        builder.HasQueryFilter(q => !q.DeletedDate.HasValue);
    }
}
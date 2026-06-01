using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Security.Entities;

namespace Persistence.EntityConfigurations;

public class OtpAuthenticatorConfiguration : IEntityTypeConfiguration<OtpAuthenticator>
{
    public void Configure(EntityTypeBuilder<OtpAuthenticator> builder)
    {
        builder.ToTable("OtpAuthenticators").HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("Id").IsRequired();
        builder.Property(x => x.UserId).HasColumnName("UserId").IsRequired();
        builder.Property(x => x.SecretKey).HasColumnName("SecretKey").IsRequired().HasMaxLength(500);
        builder.Property(x => x.IsVerified).HasColumnName("IsVerified").IsRequired();
        builder.Property(x => x.CreatedDate).HasColumnName("CreatedAt").IsRequired();
        builder.Property(x => x.UpdatedDate).HasColumnName("UpdatedAt");
        builder.Property(x => x.DeletedDate).HasColumnName("DeletedAt");

        builder.HasQueryFilter(x => !x.DeletedDate.HasValue);

        builder.HasOne(x => x.User)
            .WithMany(x => x.OtpAuthenticators)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

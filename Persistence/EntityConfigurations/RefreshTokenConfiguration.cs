using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Security.Entities;

namespace Persistence.EntityConfigurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens").HasKey(x => x.Id);
        
        builder.Property(x => x.Id).HasColumnName("Id").IsRequired();
        builder.Property(x => x.UserId).HasColumnName("UserId").IsRequired();
        builder.Property(x => x.Token).HasColumnName("Token").IsRequired().HasMaxLength(500);
        builder.Property(x => x.Expires).HasColumnName("Expires").IsRequired();
        builder.Property(x => x.CreatedIp).HasColumnName("CreatedByIp").IsRequired();
        builder.Property(x => x.Revoked).HasColumnName("Revoked");
        builder.Property(x => x.RevokedIp).HasColumnName("RevokedByIp");
        builder.Property(x => x.ReplacedToken).HasColumnName("ReplacedByToken");
        builder.Property(x => x.RevokeReason).HasColumnName("RevokeReason").HasMaxLength(500);
        builder.Property(x => x.CreatedDate).HasColumnName("CreatedAt").IsRequired();
        builder.Property(x => x.UpdatedDate).HasColumnName("UpdatedAt");
        builder.Property(x => x.DeletedDate).HasColumnName("DeletedAt");
        
        builder.HasQueryFilter(x => !x.DeletedDate.HasValue);
 
        builder.HasOne(x => x.User)
            .WithMany(x => x.RefreshTokens)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Security.Entities;

namespace Persistence.EntityConfigurations;

public class UserOperationClaimConfiguration : IEntityTypeConfiguration<UserOperationClaim>
{
    public void Configure(EntityTypeBuilder<UserOperationClaim> builder)
    {
        builder.ToTable("UserOperationClaims").HasKey(x => x.Id);
 
        builder.Property(x => x.Id).HasColumnName("Id").IsRequired();
        builder.Property(x => x.UserId).HasColumnName("UserId").IsRequired();
        builder.Property(x => x.OperationClaimId).HasColumnName("OperationClaimId").IsRequired();
        builder.Property(x => x.CreatedDate).HasColumnName("CreatedAt").IsRequired();
        builder.Property(x => x.UpdatedDate).HasColumnName("UpdatedAt");
        builder.Property(x => x.DeletedDate).HasColumnName("DeletedAt");
        
        builder.HasQueryFilter(x => !x.DeletedDate.HasValue);

        builder.HasOne(x => x.User)
            .WithMany(x => x.UserOperationClaims)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.OperationClaim)
            .WithMany(x => x.UserOperationClaims)
            .HasForeignKey(x => x.OperationClaimId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(GetSeeds());
    }

    private IEnumerable<UserOperationClaim> GetSeeds()
    {
        List<UserOperationClaim> userOperationClaims = new List<UserOperationClaim>();
        
        UserOperationClaim adminUserOperationClaim = new UserOperationClaim
        {
            Id = 1,
            UserId = 1, // Admin user
            OperationClaimId = 1 // Admin claim
        };

        userOperationClaims.Add(adminUserOperationClaim);

        return userOperationClaims;
    }
}

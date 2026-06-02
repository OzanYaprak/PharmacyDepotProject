using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Security.Entities;

namespace Persistence.EntityConfigurations;

public class OperationClaimConfiguration : IEntityTypeConfiguration<OperationClaim>
{
    public void Configure(EntityTypeBuilder<OperationClaim> builder)
    {
        builder.ToTable("OperationClaims").HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("Id").IsRequired();
        builder.Property(x => x.Name).HasColumnName("Name").IsRequired().HasMaxLength(100);
        builder.Property(x => x.CreatedDate).HasColumnName("CreatedAt").IsRequired();
        builder.Property(x => x.UpdatedDate).HasColumnName("UpdatedAt");
        builder.Property(x => x.DeletedDate).HasColumnName("DeletedAt");

        builder.HasQueryFilter(x => !x.DeletedDate.HasValue);

        builder.HasMany(x => x.UserOperationClaims)
            .WithOne(x => x.OperationClaim)
            .HasForeignKey(x => x.OperationClaimId)
            .OnDelete(DeleteBehavior.Cascade);

        //builder.HasData(GetSeeds());
    }

    //private IEnumerable<OperationClaim> _seedData
    //{
    //    get
    //    {
    //        int id = 0;

    //        yield return new OperationClaim
    //        {
    //            Id = ++id,
    //            Name = GeneralOperationClaims.Admin
    //        };

    //        IEnumerable<Type> featureOperationClaimsTypes = Assembly
    //            .GetAssembly(typeof(ApplicationServiceRegistration))!.GetTypes()
    //            .Where(t =>
    //            (t.Namespace?.Contains("Features") == true)
    //            &&
    //            (t.Namespace?.Contains("Constants") == true)
    //            &&
    //            t.IsClass
    //            &&
    //            t.Name.EndsWith("OperationClaims"));

    //        foreach (Type featureOperationClaimsType in featureOperationClaimsTypes)
    //        {
    //            FieldInfo[] typeFields = featureOperationClaimsType.GetFields(BindingFlags.Public | BindingFlags.Static);
    //            IEnumerable<string> typeFieldsValues = typeFields.Select(f => f.GetValue(null)!.ToString()!);

    //            IEnumerable<OperationClaim> featureOperationClaimsToAdd = typeFieldsValues.Select(value => new OperationClaim
    //            {
    //                Id = ++id,
    //                Name = value
    //            });

    //            foreach (OperationClaim featureOperationClaim in featureOperationClaimsToAdd)
    //            {
    //                yield return featureOperationClaim;
    //            }
    //        }
    //    }
    //}
}

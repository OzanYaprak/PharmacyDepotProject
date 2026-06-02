using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Security.Entities;
using Security.Hashing;
using System.Reflection;

namespace Persistence.EntityConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users").HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("Id").IsRequired();
        builder.Property(x => x.FirstName).HasColumnName("FirstName").IsRequired().HasMaxLength(75);
        builder.Property(x => x.LastName).HasColumnName("LastName").IsRequired().HasMaxLength(75);
        builder.Property(x => x.Email).HasColumnName("Email").IsRequired().HasMaxLength(255);
        builder.Property(x => x.PasswordSalt).HasColumnName("PasswordSalt").IsRequired();
        builder.Property(x => x.PasswordHash).HasColumnName("PasswordHash").IsRequired();
        builder.Property(x => x.IsActive).HasColumnName("IsActive").HasDefaultValue(true);
        builder.Property(x => x.AuthenticatorType).HasColumnName("AuthenticatorType").IsRequired();
        builder.Property(x => x.CreatedDate).HasColumnName("CreatedAt").IsRequired();
        builder.Property(x => x.UpdatedDate).HasColumnName("UpdatedAt");
        builder.Property(x => x.DeletedDate).HasColumnName("DeletedAt");

        builder.HasQueryFilter(x => !x.DeletedDate.HasValue);

        builder.HasMany(x => x.UserOperationClaims)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.RefreshTokens)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.OtpAuthenticators)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.EmailAuthenticators)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(GetSeeds());
    }

    private IEnumerable<User> GetSeeds()
    {
        List<User> users = new List<User>();

        HashingHelper.CreatePasswordHash(
            password: "admin",
            passwordHash: out byte[] passwordHash,
            passwordSalt: out byte[] passwordSalt
        );

        users.Add(new User
        {
            Id = 1,
            FirstName = "Admin",
            LastName = "User",
            Email = "admin@example.com",
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            IsActive = true,
            AuthenticatorType = Security.Enums.AuthenticatorType.None
        });

        return users.ToArray();
    }
}
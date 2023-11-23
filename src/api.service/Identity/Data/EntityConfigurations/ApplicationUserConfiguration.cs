using api.service.Shared.Models.IdentityModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Services.Identity.Identity.Data.EntityConfigurations;

internal class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.LastName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.UserName).HasMaxLength(50).IsRequired();
        builder.Property(x => x.NormalizedUserName).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(50).IsRequired();
        builder.Property(x => x.NormalizedEmail).HasMaxLength(50).IsRequired();
        builder.Property(x => x.PhoneNumber).HasMaxLength(15).IsRequired(false);

        builder.Property(x => x.CreatedAt).HasDefaultValueSql("now()");

        builder
            .Property(x => x.UserState)
            .HasDefaultValue(UserState.Active)
            .HasConversion(x => x.ToString(), x => (UserState)Enum.Parse(typeof(UserState), x));

        builder.HasIndex(x => x.Email).IsUnique();
        builder.HasIndex(x => x.NormalizedEmail).IsUnique();
        builder.HasMany(e => e.UserRoles).WithOne(e => e.User).HasForeignKey(ur => ur.UserId).IsRequired();
    }
}

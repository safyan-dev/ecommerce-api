using api.service.Shared.Models.IdentityModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace api.service.Identity.Data.EntityConfigurations;

public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        // https://docs.microsoft.com/en-us/aspnet/core/security/authentication/customize-identity-model#add-navigation-properties
        // Each Role can have many entries in the UserRole join table
        builder.HasMany(e => e.UserRoles).WithOne(e => e.Role).HasForeignKey(ur => ur.RoleId).IsRequired();
    }
}

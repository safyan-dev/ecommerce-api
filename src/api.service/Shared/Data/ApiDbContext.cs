using api.service.Shared.Models.IdentityModels;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace api.service.Shared.Data
{
    public class ApiDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid, IdentityUserClaim<Guid>, ApplicationUserRole, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
    {
        public ApiDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(GetType().Assembly);

            foreach (var entity in builder.Model.GetEntityTypes())
            {
                // Replace table names
                entity.SetTableName(entity.GetTableName()?.Underscore());

                var ecommerceObjectIdentifier = StoreObjectIdentifier.Table(
                    entity.GetTableName()?.Underscore()!,
                    entity.GetSchema()
                );

                // Replace column names
                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(property.GetColumnName(ecommerceObjectIdentifier)?.Underscore());
                }

                foreach (var key in entity.GetKeys())
                {
                    key.SetName(key.GetName()?.Underscore());
                }

                foreach (var key in entity.GetForeignKeys())
                {
                    key.SetConstraintName(key.GetConstraintName()?.Underscore());
                }
            }
        }
    }
}

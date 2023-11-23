using api.service.Identity;
using Microsoft.AspNetCore.Identity;
using System.Globalization;

namespace api.service.Shared.Models.IdentityModels
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = default!;
        public static ApplicationRole User => new()
        {
            Name = IdentityServerConstants.Role.User,
            NormalizedName = nameof(User).ToUpper(CultureInfo.InvariantCulture),
        };

        public static ApplicationRole Admin => new()
        {
            Name = IdentityServerConstants.Role.Admin,
            NormalizedName = nameof(Admin).ToUpper(CultureInfo.InvariantCulture)
        };
    }
}

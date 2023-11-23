using Microsoft.AspNetCore.Identity;

namespace api.service.Shared.Models.IdentityModels
{
    public class ApplicationUserRole : IdentityUserRole<Guid>
    {
        public virtual ApplicationUser? User { get; set; }
        public virtual ApplicationRole? Role { get; set; }
    }
}

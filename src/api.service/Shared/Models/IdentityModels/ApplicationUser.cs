using Microsoft.AspNetCore.Identity;

namespace api.service.Shared.Models.IdentityModels
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public DateTime? LastLoggedInAt { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = default!;
        public virtual ICollection<AccessToken> AccessTokens { get; set; } = default!;
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = default!;
        public UserState UserState { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
using System.Security.Claims;

namespace api.service.Shared.Models.IdentityModels
{
    public class ClaimPolicy
    {
        public ClaimPolicy(string name, IReadOnlyList<Claim>? claims)
        {
            Name = name;
            Claims = claims ?? new List<Claim>();
        }

        public string Name { get; set; }
        public IReadOnlyList<Claim> Claims { get; set; }
    }
}

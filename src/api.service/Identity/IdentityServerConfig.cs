using Duende.IdentityServer.Models;
using Duende.IdentityServer;
using IdentityModel;

namespace api.service.Identity
{
    public class IdentityServerConfig
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                    new IdentityResources.OpenId(),
                    new IdentityResources.Profile(),
                    new IdentityResources.Email(),
                    new IdentityResources.Phone(),
                    new IdentityResources.Address(),
                    new("roles", "User Roles", new List<string> { "role" })
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope> { new("api-example", "Web API") };

        public static IList<ApiResource> ApiResources =>
            new List<ApiResource>
            {
            new ApiResource("api-example-resources", "Web API Resource")
            {
                Scopes = { "api-example" },
                UserClaims = { JwtClaimTypes.Role, JwtClaimTypes.Name, JwtClaimTypes.Id }
            }
            };

        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                new()
                {
                    ClientId = "frontend-client",
                    ClientName = "Frontend Client",
                    RequireClientSecret = false,
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowedScopes =
                    {
                        Duende.IdentityServer.IdentityServerConstants.StandardScopes.OpenId,
                        Duende.IdentityServer.IdentityServerConstants.StandardScopes.Profile,
                        Duende.IdentityServer.IdentityServerConstants.StandardScopes.Email,
                        "roles",
                        "api-example"
                    }
                },
                new()
                {
                    ClientId = "oauthClient",
                    ClientName = "Example client application using client credentials",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = new List<Secret> { new("SuperSecretPassword".Sha256()) },
                    AllowedScopes =
                    {
                        Duende.IdentityServer.IdentityServerConstants.StandardScopes.OpenId,
                        Duende.IdentityServer.IdentityServerConstants.StandardScopes.Profile,
                        Duende.IdentityServer.IdentityServerConstants.StandardScopes.Email,
                        "roles",
                        "api-example"
                    }
                }
            };
    }
}

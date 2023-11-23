using api.service.Identity;
using api.service.Identity.Features.GettingClaims.v1;
using api.service.Identity.Features.Login.v1;
using api.service.Identity.Features.RefreshingToken.v1;
using api.service.Users.Features.GettingUsers.v1;
using Asp.Versioning.Builder;
using ECommerce.Services.Identity.Identity.Features.Logout.v1;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace api_example.service.Shared.Extensions.WebApplicationBuilderExtensions
{
    public static partial class WebApplicationExtensions
    {
        public const string IdentityTag = "Identity";
        public const string IdentityPrefixUri = "api/v{version:apiVersion}/identity";

        public const string UserTag = "Identity";
        public const string UserPrefixUri = "api/v{version:apiVersion}/user";

        public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder builder)
        {
            var version = builder.NewVersionedApi();
            MapIdentityEndpoints(builder);
            MapUserEndpoints(version);

            return builder;
        }

        private static void MapUserEndpoints(IVersionedEndpointRouteBuilder version)
        {
            var userV1 = version.MapGroup(UserPrefixUri).HasApiVersion(1.0);
            userV1.MapGetUsersEndpoint();
        }

        private static void MapIdentityEndpoints(this IEndpointRouteBuilder builder)
        {
            var version = builder.NewVersionedApi();
            var identityV1 = version.MapGroup(IdentityPrefixUri).HasApiVersion(1.0);
            identityV1.MapGet("/user-role", [Authorize(
                                    AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
                                    Roles = IdentityServerConstants.Role.User
                                )]
            () => new { Role = IdentityServerConstants.Role.User })
                            .WithTags(IdentityTag);

            identityV1.MapGet("/admin-role",
                    [Authorize(
                        AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
                        Roles = IdentityServerConstants.Role.Admin
                    )]
            () => new { Role = IdentityServerConstants.Role.Admin })
                .WithTags(IdentityTag);

            identityV1.MapLoginUserEndpoint();
            identityV1.MapLogoutEndpoint();
            identityV1.MapRefreshTokenEndpoint();
            identityV1.MapGetClaimsEndpoint();
        }
    }
}

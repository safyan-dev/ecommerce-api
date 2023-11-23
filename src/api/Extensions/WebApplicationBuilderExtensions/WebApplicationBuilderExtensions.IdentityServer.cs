using api.service.Identity;
using api.service.Identity.Services;
using api.service.Shared.Models.IdentityModels;

namespace api_example.Extensions.WebApplicationBuilderExtensions
{
    public static partial class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder AddCustomIdentityServer(this WebApplicationBuilder builder)
        {
            builder.Services.AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                })
                .AddInMemoryIdentityResources(IdentityServerConfig.IdentityResources)
                .AddInMemoryApiResources(IdentityServerConfig.ApiResources)
                .AddInMemoryApiScopes(IdentityServerConfig.ApiScopes)
                .AddInMemoryClients(IdentityServerConfig.Clients)
                .AddAspNetIdentity<ApplicationUser>()
                .AddProfileService<IdentityProfileService>()
                .AddDeveloperSigningCredential(); // This is for dev only scenarios when you don’t have a certificate to use.;

            return builder;
        }
    }
}

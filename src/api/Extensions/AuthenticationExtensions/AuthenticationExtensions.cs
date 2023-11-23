using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using api_example.Extensions.ConfigurationExtensions;
using api.service.Shared.Models;
using api.service.Security;
using api.service.Shared.Models.IdentityModels;

namespace api_example.Extensions.AuthenticationExtensions
{
    public static partial class AuthenticationExtensions
    {
        public static AuthenticationBuilder AddCustomJwtAuthentication(this IServiceCollection services, IConfiguration configuration,Action<JwtOptions>? optionConfigurator = null)
        {
            // need to learn about them
            //JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            //JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

            AddJwtServices(services, configuration, optionConfigurator);

            var jwtOptions = configuration.BindOptions<JwtOptions>(nameof(JwtOptions));
            Guard.Against.Null(jwtOptions, nameof(jwtOptions));

            return services
                .AddAuthentication() // no default scheme specified
                .AddJwtBearer(options =>
                {
                    //-- JwtBearerDefaults.AuthenticationScheme --
                    options.Audience = jwtOptions.Audience;
                    options.SaveToken = true;
                    options.RefreshOnIssuerKeyNotFound = false;
                    options.RequireHttpsMetadata = false;
                    options.IncludeErrorDetails = true;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidAudience = jwtOptions.Audience,
                        SaveSigninToken = true,
                        ClockSkew = TimeSpan.Zero,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
                    };
                });
        }

        public static IServiceCollection AddJwtServices(this IServiceCollection services,IConfiguration configuration, Action<JwtOptions>? optionConfigurator = null)
        {
            var jwtOptions = configuration.BindOptions<JwtOptions>(nameof(JwtOptions));
            Guard.Against.Null(jwtOptions, nameof(jwtOptions));

            optionConfigurator?.Invoke(jwtOptions);

            if (optionConfigurator is { })
            {
                services.Configure(nameof(JwtOptions), optionConfigurator);
            }
            else
            {
                services
                    .AddOptions<JwtOptions>()
                    .Bind(configuration.GetSection(nameof(JwtOptions)))
                    .ValidateDataAnnotations();
            }

            services.AddTransient<JwtService>();

            return services;
        }

        public static IServiceCollection AddCustomAuthorization(this IServiceCollection services, IList<ClaimPolicy>? claimPolicies = null, IList<RolePolicy>? rolePolicies = null)
        {
            services.AddAuthorization(authorizationOptions =>
            {
                var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
                    JwtBearerDefaults.AuthenticationScheme
                );

                defaultAuthorizationPolicyBuilder = defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
                authorizationOptions.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();

                if (claimPolicies is { })
                {
                    foreach (var policy in claimPolicies)
                    {
                        authorizationOptions.AddPolicy(
                            policy.Name,
                            x =>
                            {
                                x.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                                foreach (var policyClaim in policy.Claims)
                                {
                                    x.RequireClaim(policyClaim.Type, policyClaim.Value);
                                }
                            }
                        );
                    }
                }

                if (rolePolicies is { })
                {
                    foreach (var rolePolicy in rolePolicies)
                    {
                        authorizationOptions.AddPolicy(
                            rolePolicy.Name,
                            x =>
                            {
                                x.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                                x.RequireRole(rolePolicy.Roles);
                            }
                        );
                    }
                }
            });

            return services;
        }
    }
}

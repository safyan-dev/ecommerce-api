using api.building.Persistence;
using api.service.Identity;
using api.service.Identity.Data;
using api.service.Shared.Models.IdentityModels;
using api_example.ConfigurationOptions;
using api_example.Extensions.AuthenticationExtensions;
using api_example.Extensions.ConfigurationExtensions;
using api_example.Extensions.SwaggerExtensions;
using api_example.Extensions.WebApplicationBuilderExtensions;
using api_example.service.Shared.Extensions.VersioningExtensions;
using FluentValidation;
using System.Reflection;
using System.Threading.RateLimiting;

namespace api_example.service.Shared.Extensions.WebApplicationBuilderExtensions
{
    public static partial class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
        {
            builder.Services.AddAntiforgery();

            builder.Services.AddCustomJwtAuthentication(builder.Configuration);

            builder.Services.AddCustomAuthorization(
                rolePolicies: new List<RolePolicy>
                {
                    new(IdentityServerConstants.Role.Admin, new List<string> { IdentityServerConstants.Role.Admin }),
                    new(IdentityServerConstants.Role.User, new List<string> { IdentityServerConstants.Role.User })
                }
            );

            builder.AddCustomProblemDetails();

            builder.AddCustomSerilog();

            builder.AddCustomVersioning();

            builder.AddCustomSwagger();

            builder.AddCustomHealthCheck(healthChecksBuilder =>
            {
                var postgresOptions = builder.Configuration.BindOptions<PostgreSqlOptions>();

                healthChecksBuilder.AddNpgSql(
                        postgresOptions.ConnectionString,
                        name: "Postgres-Check",
                        tags: new[] { "postgres", "database", "infra", "api", "live", "ready" });
            });

            builder.AddCustomIdentity();

            builder.AddCustomIdentityServer();

            builder.Services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
                    httpContext =>
                        RateLimitPartition.GetFixedWindowLimiter(
                            partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
                            factory: partition =>
                                new FixedWindowRateLimiterOptions
                                {
                                    AutoReplenishment = true,
                                    PermitLimit = 100,
                                    QueueLimit = 0,
                                    Window = TimeSpan.FromMinutes(1)
                                }
                        )
                );
            });

            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            builder.Services.AddScoped<IDataSeeder, IdentityDataSeeder>();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly()); // to do
            builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

            return builder;
        }
    }
}

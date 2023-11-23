using api.service.Shared.Data;
using api.service.Shared.Models.IdentityModels;
using api_example.ConfigurationOptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api_example.Extensions.WebApplicationBuilderExtensions
{
    public static partial class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder AddCustomIdentity(this WebApplicationBuilder builder, Action<IdentityOptions>? configure = null)
        {
            if (builder.Configuration.GetValue<bool>("PostgreSqlOptions:UseInMemory"))
            {
                builder.Services.AddDbContext<ApiDbContext>(options => options.UseInMemoryDatabase("api-example"));
            }
            else
            {
                // Sql server
                var postgresqlOptions = new PostgreSqlOptions();
                builder.Configuration.GetSection("PostgreSqlOptions").Bind(postgresqlOptions);
                builder.Services.AddDbContext<ApiDbContext>((sp, options) => 
                {
                    options.UseNpgsql(postgresqlOptions.ConnectionString, postgresqlOptions =>
                    {
                        var name = typeof(ApiDbContext).Assembly.GetName().Name;

                        postgresqlOptions.MigrationsAssembly(name);
                        postgresqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                    });
                });
            }

            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 3;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.RequireUniqueEmail = true;

                if (configure is { })
                    configure.Invoke(options);
            })
            .AddEntityFrameworkStores<ApiDbContext>()
            .AddDefaultTokenProviders();

            if (builder.Configuration.GetSection(nameof(IdentityOptions)) is not null)
                builder.Services.Configure<IdentityOptions>(builder.Configuration.GetSection(nameof(IdentityOptions)));

            return builder;
        }
    }
}

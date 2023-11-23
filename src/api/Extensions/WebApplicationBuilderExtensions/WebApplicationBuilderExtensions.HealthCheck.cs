using api_example.ConfigurationOptions;
using api_example.Extensions.ConfigurationExtensions;

namespace api_example.Extensions.WebApplicationBuilderExtensions
{
    public static partial class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder AddCustomHealthCheck(this WebApplicationBuilder builder,Action<IHealthChecksBuilder>? healthChecksBuilder = null,string sectionName = nameof(HealthOptions))
        {
            var healthOptions = builder.Configuration.BindOptions<HealthOptions>(sectionName);
            if (!healthOptions.Enabled)
            {
                return builder;
            }

            var healCheckBuilder = builder.Services.AddHealthChecks();

            healthChecksBuilder?.Invoke(healCheckBuilder);

            builder.Services.AddHealthChecksUI(setup =>
                {
                    setup.SetEvaluationTimeInSeconds(60); // time in seconds between check
                    setup.AddHealthCheckEndpoint("All Checks", "/healthz");
                    setup.AddHealthCheckEndpoint("Infra", "/health/infra");
                    //setup.AddHealthCheckEndpoint("Bus", "/health/bus"); // need to learn about it
                    setup.AddHealthCheckEndpoint("Database", "/health/database");
                    //setup.AddHealthCheckEndpoint("Downstream Services", "/health/downstream-services"); // need to learn about it
                }).AddInMemoryStorage();

            return builder;
        }
    }
}

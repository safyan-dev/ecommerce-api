using api_example.ConfigurationOptions;
using api_example.Extensions.ConfigurationExtensions;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace api_example.service.Shared.Extensions.WebApplicationExtensions
{
    public static partial class WebApplicationExtensions
    {
        public static WebApplication UseCustomHealthCheck(this WebApplication app)
        {
            var healthOptions = app.Configuration.BindOptions<HealthOptions>();
            if (!healthOptions.Enabled)
            {
                return app;
            }

            // need to learn, used Prometheus metrics package
            //app.UseHttpMetrics();
            //app.UseGrpcMetrics();

            app.UseHealthChecks(
                    "/healthz",
                    new HealthCheckOptions
                    {
                        Predicate = _ => true,
                        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
                        ResultStatusCodes =
                        {
                            [HealthStatus.Healthy] = StatusCodes.Status200OK,
                            [HealthStatus.Degraded] = StatusCodes.Status500InternalServerError,
                            [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable,
                        },
                    }
                )
                .UseHealthChecks(
                    "/health/database",
                    new HealthCheckOptions
                    {
                        Predicate = check => check.Tags.Contains("database"),
                        AllowCachingResponses = false,
                        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
                    }
                );

            return app;
        }
    }
}

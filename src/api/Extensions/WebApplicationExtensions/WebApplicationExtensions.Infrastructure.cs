using api_example.Extensions.SwaggerExtensions;
using api_example.Extensions.WebApplicationExtensions;
using api_example.service.Shared.Extensions.WebApplicationBuilderExtensions;
using Serilog;

namespace api_example.service.Shared.Extensions.WebApplicationExtensions
{
    public static partial class WebApplicationExtensions
    {
        public static WebApplication UseInfrastructure(this WebApplication app)
        {
            app.UseAntiforgery();

            app.UseAppCors();

            app.UseSerilogRequestLogging();
            
            app.UseIdentityServer();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRateLimiter();

            app.UseCustomSwagger();

            app.MapControllers();

            app.MapEndpoints();

            app.UseCustomHealthCheck();

            return app;
        }
    }
}

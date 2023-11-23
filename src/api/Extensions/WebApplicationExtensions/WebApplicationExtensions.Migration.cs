using api.service.Shared.Data;
using Microsoft.EntityFrameworkCore;

namespace api_example.Extensions.WebApplicationExtensions
{
    public static partial class WebApplicationExtensions
    {
        public static async Task ApplyDatabaseMigrations(this WebApplication app)
        {
            var configuration = app.Services.GetRequiredService<IConfiguration>();
            if (configuration.GetValue<bool>("PostgreSqlOptions:UseInMemory") == false)
            {
                using var serviceScope = app.Services.CreateScope();
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApiDbContext>();

                app.Logger.LogInformation("Updating database...");

                await dbContext.Database.MigrateAsync();

                app.Logger.LogInformation("Updated database");
            }
        }
    }
}

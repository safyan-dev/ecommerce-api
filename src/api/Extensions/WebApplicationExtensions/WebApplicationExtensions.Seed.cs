using api.building.Persistence;
using System.Data;

namespace api_example.Extensions.WebApplicationExtensions
{
    public static partial class WebApplicationExtensions
    {
        public static async Task SeedData(this WebApplication app)
        {
            if (!app.Environment.IsEnvironment("test"))
            {
                using var serviceScope = app.Services.CreateScope();
                var seeders = serviceScope.ServiceProvider.GetServices<IDataSeeder>();

                foreach (var seeder in seeders.OrderBy(x => x.Order))
                {
                    app.Logger.LogInformation("Seeding '{Seed}' started...", seeder.GetType().Name);
                    await seeder.SeedAllAsync();
                    app.Logger.LogInformation("Seeding '{Seed}' ended...", seeder.GetType().Name);
                }
            }
        }
    }
}

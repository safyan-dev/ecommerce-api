using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace api.service.Shared.Data
{
    public class DbContextDesignFactory : IDesignTimeDbContextFactory<ApiDbContext>
    {
        private readonly string _connectionStringSection;

        public DbContextDesignFactory()
        {
            _connectionStringSection = "PostgreSqlOptions:ConnectionString";
        }

        public ApiDbContext CreateDbContext(string[] args)
        {
            Console.WriteLine($"BaseDirectory: {AppContext.BaseDirectory}");

            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "test";

            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory ?? "")
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environmentName}.json", true) // it is optional
                .AddEnvironmentVariables();

            var configuration = builder.Build();

            var connectionStringSectionValue = configuration.GetValue<string>(_connectionStringSection);

            if (string.IsNullOrWhiteSpace(connectionStringSectionValue))
            {
                throw new InvalidOperationException($"Could not find a value for {_connectionStringSection} section.");
            }

            Console.WriteLine($"ConnectionString  section value is : {connectionStringSectionValue}");

            var optionsBuilder = new DbContextOptionsBuilder<ApiDbContext>()
                .UseNpgsql(
                    connectionStringSectionValue,
                    postgreSqlOptions =>
                    {
                        postgreSqlOptions.MigrationsAssembly(GetType().Assembly.FullName);
                        postgreSqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
                    }
                );

            return (ApiDbContext)Activator.CreateInstance(typeof(ApiDbContext), optionsBuilder.Options);
        }
    }
}

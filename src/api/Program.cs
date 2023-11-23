using api_example.Extensions.WebApplicationExtensions;
using api_example.service.Shared.Extensions.WebApplicationBuilderExtensions;
using api_example.service.Shared.Extensions.WebApplicationExtensions;

var builder = WebApplication.CreateSlimBuilder(args);
builder.AddInfrastructure();

var app = builder.Build();

app.UseInfrastructure();

await app.ApplyDatabaseMigrations();
await app.SeedData();

app.Run();
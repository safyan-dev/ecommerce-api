using Asp.Versioning;

namespace api_example.service.Shared.Extensions.VersioningExtensions
{
    public static partial class VersioningExtensions
    {
        public static WebApplicationBuilder AddCustomVersioning(this WebApplicationBuilder builder, Action<ApiVersioningOptions>? configurator = null)
        {
            builder.Services.AddApiVersioning(options =>
                {
                    options.ReportApiVersions = true;
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.DefaultApiVersion = new ApiVersion(1, 0);
                    options.ApiVersionReader = ApiVersionReader.Combine(
                        new HeaderApiVersionReader("api-version"),
                        new QueryStringApiVersionReader(),
                        new UrlSegmentApiVersionReader()
                    );

                    configurator?.Invoke(options);
                })
                .AddApiExplorer(options =>
                {
                    options.SubstituteApiVersionInUrl = true;
                });

            return builder;
        }
    }
}

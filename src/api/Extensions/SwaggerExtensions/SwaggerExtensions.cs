using Asp.Versioning.ApiExplorer;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using System.Text;
using Swashbuckle.AspNetCore.SwaggerGen;
using api_example.Extensions.ConfigurationExtensions;
using api_example.ConfigurationOptions;

namespace api_example.Extensions.SwaggerExtensions
{
    public static partial class SwaggerExtensions
    {
        public static WebApplicationBuilder AddCustomSerilog(this WebApplicationBuilder builder,string sectionName = "Serilog",Action<LoggerConfiguration>? extraConfigure = null)
        {
            var serilogOptions = builder.Configuration.BindOptions<SerilogOptions>(sectionName);

            builder.Host.UseSerilog((context, serviceProvider, loggerConfiguration) =>
                {
                    extraConfigure?.Invoke(loggerConfiguration);
                });

            return builder;
        }

        public static WebApplicationBuilder AddCustomSwagger(this WebApplicationBuilder builder, params Assembly[] assemblies)
        {
            var scanAssemblies = new[] { Assembly.GetExecutingAssembly() };

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            builder.Services.AddSingleton<SwaggerOptions>();

            builder.Services.AddSwaggerGen(options =>
            {
                foreach (var assembly in scanAssemblies)
                {
                    var xmlFile = XmlCommentsFilePath(assembly);
                    if (File.Exists(xmlFile))
                        options.IncludeXmlComments(xmlFile);
                }

                var bearerScheme = new OpenApiSecurityScheme()
                {
                    Type = SecuritySchemeType.Http,
                    Name = JwtBearerDefaults.AuthenticationScheme,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    Reference = new() { Type = ReferenceType.SecurityScheme, Id = JwtBearerDefaults.AuthenticationScheme }
                };

                var apiKeyScheme = new OpenApiSecurityScheme
                {
                    Description = "Api key needed to access the endpoints. X-Api-Key: My_API_Key",
                    In = ParameterLocation.Header,
                    Name = Constants.ApiKeyConstants.HeaderName,
                    Scheme = Constants.ApiKeyConstants.DefaultScheme,
                    Type = SecuritySchemeType.ApiKey,
                    Reference = new() { Type = ReferenceType.SecurityScheme, Id = Constants.ApiKeyConstants.HeaderName }
                };

                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, bearerScheme);

                options.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        { bearerScheme, Array.Empty<string>() },
                        { apiKeyScheme, Array.Empty<string>() }
                    }
                );

                options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            });

            static string XmlCommentsFilePath(Assembly assembly)
            {
                var basePath = Path.GetDirectoryName(assembly.Location);
                var fileName = assembly.GetName().Name + ".xml";
                return Path.Combine(basePath, fileName);
            }

            return builder;
        }

        public static IApplicationBuilder UseCustomSwagger(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                var descriptions = app.DescribeApiVersions();

                // build a swagger endpoint for each discovered API version
                foreach (var description in descriptions)
                {
                    var url = $"/swagger/{description.GroupName}/swagger.json";
                    var name = description.GroupName.ToUpperInvariant();
                    options.SwaggerEndpoint(url, name);
                }
            });

            return app;
        }
    }


    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider provider;
        private readonly SwaggerOptions? _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigureSwaggerOptions"/> class.
        /// </summary>
        /// <param name="provider">The <see cref="IApiVersionDescriptionProvider">provider</see> used to generate Swagger documents.</param>
        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider, IOptions<SwaggerOptions> options)
        {
            this.provider = provider;
            _options = options.Value;
        }

        /// <inheritdoc />
        public void Configure(SwaggerGenOptions options)
        {
            // add a swagger document for each discovered API version
            // note: you might choose to skip or document deprecated API versions differently
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
            }
        }

        private OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var text = new StringBuilder("An example application with OpenAPI, Swashbuckle, and API versioning.");
            var info = new OpenApiInfo
            {
                Version = description.ApiVersion.ToString(),
                Title = _options?.Title ?? "APIs",
                Description = "An application with Swagger, Swashbuckle, and API versioning."
            };

            if (description.IsDeprecated)
            {
                text.Append("This API version has been deprecated.");
            }

            if (description.SunsetPolicy is SunsetPolicy policy)
            {
                if (policy.Date is DateTimeOffset when)
                {
                    text.Append(" The API will be sunset on ").Append(when.Date.ToShortDateString()).Append('.');
                }

                if (policy.HasLinks)
                {
                    text.AppendLine();

                    for (var i = 0; i < policy.Links.Count; i++)
                    {
                        var link = policy.Links[i];

                        if (link.Type == "text/html")
                        {
                            text.AppendLine();

                            if (link.Title.HasValue)
                            {
                                text.Append(link.Title.Value).Append(": ");
                            }

                            text.Append(link.LinkTarget.OriginalString);
                        }
                    }
                }
            }

            info.Description = text.ToString();

            return info;
        }
    }

    public class SwaggerOptions
    {
        public string Title { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
    }
}

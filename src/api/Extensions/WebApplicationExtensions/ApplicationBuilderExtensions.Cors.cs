namespace api_example.Extensions.WebApplicationExtensions
{
    public static partial class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseAppCors(this IApplicationBuilder app)
        {
            app.UseCors(p =>
            {
                p.AllowAnyOrigin();
                p.AllowAnyMethod();
                p.AllowAnyHeader();
            });

            return app;
        }
    }
}

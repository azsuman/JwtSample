using Microsoft.AspNetCore.Authentication.JwtBearer;
using NSwag;

namespace JwtSample.OpenApi;

internal static class Startup
{
    private static SwaggerSettings? _settings;

    internal static IServiceCollection AddOpenApiDocumentation(this IServiceCollection services, IConfiguration config)
    {
        _settings = config.GetSection(nameof(SwaggerSettings)).Get<SwaggerSettings>();
        services.AddEndpointsApiExplorer();

        _ = services.AddOpenApiDocument((document, serviceProvider) =>
        {
            document.PostProcess = doc =>
            {
                doc.Info.Title = _settings.Title;
                doc.Info.Version = _settings.Version;
                doc.Info.Description = _settings.Description;
                doc.Info.Contact = new()
                {
                    Name = _settings.ContactName,
                    Email = _settings.ContactEmail,
                    Url = _settings.ContactUrl
                };
            };

            _ = document.AddSecurity(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Input Bearer token into the textbox: {JWT token}.",
                In = OpenApiSecurityApiKeyLocation.Header,
                Type = OpenApiSecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                BearerFormat = "JWT",
            });

            document.OperationProcessors.Add(new AuthOperationProcessor());
        });

        return services;
    }

    internal static IApplicationBuilder UseOpenApiDocumentation(this IApplicationBuilder app)
    {
        app.UseOpenApi();
        app.UseSwaggerUi3(options =>
        {
            options.DefaultModelsExpandDepth = -1;
            options.DocumentTitle = _settings?.Title;
        });

        return app;
    }
}
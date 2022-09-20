using JwtSample.Auth.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace JwtSample.Auth;

internal static class Startup
{
    internal static IServiceCollection AddJwtAuth(this IServiceCollection services)
    {
        _ = services.AddOptions<SecuritySettings>()
           .BindConfiguration(SecuritySettings.ConfigSectionPath);

        _ = services.AddOptions<JwtSettings>()
            .BindConfiguration(JwtSettings.ConfigSectionPath)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>();

        _ = services.AddAuthentication(authentication =>
        {
            authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, configureOptions: null!);

        return services;
    }
}

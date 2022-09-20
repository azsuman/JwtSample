using JwtSample.Auth;
using JwtSample.Common;
using JwtSample.Data;
using JwtSample.Middleware;
using JwtSample.OpenApi;
using JwtSample.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

StaticLogger.EnsureInitialized();
Log.Information("Server Booting Up...");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((_, config) =>
    {
        config.WriteTo.Console();
    });

    builder.Services.AddControllers();
    builder.Services.AddJwtAuth();
    builder.Services.AddExceptionMiddleware();
    builder.Services.AddOpenApiDocumentation(builder.Configuration);
    builder.Services.AddDbContext<DataContext>(options => options.UseInMemoryDatabase("SampleJwt"));
    builder.Services.AddServices();

    var app = builder.Build();
    app.UseExceptionMiddleware();
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseOpenApiDocumentation();
    app.AddUsers();
    app.MapControllers().RequireAuthorization();

    app.Run();
}
catch (Exception ex)
{
    StaticLogger.EnsureInitialized();
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    StaticLogger.EnsureInitialized();
    Log.Information("Server Shutting down...");
    Log.CloseAndFlush();
}
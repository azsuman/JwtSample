using JwtSample.Entities;
using static JwtSample.Common.MOneCrypto;

namespace JwtSample.Data;

public static class DbInitializer
{
    public static void SeedDefaultUsers(this WebApplication app)
    {
        var scope = app.Services.CreateScope();

        using var dataContext = scope.ServiceProvider.GetService<DataContext>();
        
        if (dataContext != null)
        {
            dataContext.Database.EnsureCreated();

            if (!dataContext.Users.Any())
            {
                dataContext.AddRange(DefaultUsers());

                dataContext?.SaveChanges();
            }
        }
    }

    private static List<User> DefaultUsers()
    {
        CreatePasswordHash("@bc123", out byte[] passwordHash, out byte[] passwordSalt);

        return new List<User>
        {
            new User
            {
                UserName = "Admin",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                RefreshToken = Guid.NewGuid().ToString(),
                TokenCreated = DateTime.Now,
                TokenExpiration = DateTime.Now.AddMinutes(30),
                DateCreated = DateTime.Now,
                IsApproved = true
            },
            new User
            {
                UserName = "MOne",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                RefreshToken = Guid.NewGuid().ToString(),
                TokenCreated = DateTime.Now,
                TokenExpiration = DateTime.Now.AddMinutes(30),
                IsApproved = true
            }
        };

    }
}

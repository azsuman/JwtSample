using JwtSample.Entities;
using static JwtSample.Common.MOneCrypto;

namespace JwtSample.Data;

public static class SeedData
{
    public static void AddUsers(this WebApplication app)
    {
        var scope = app.Services.CreateScope();
        var dataContext = scope.ServiceProvider.GetService<DataContext>();

        dataContext?.AddRange(GetUsers());

        dataContext?.SaveChanges();
    }

    private static List<User> GetUsers()
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

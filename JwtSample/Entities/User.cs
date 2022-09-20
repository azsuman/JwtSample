using System.ComponentModel.DataAnnotations;

namespace JwtSample.Entities;
#nullable disable
public class User
{
    [Key]
    public string UserName { get; set; } = default!;

    public byte[] PasswordHash { get; set; }

    public byte[] PasswordSalt { get; set; }

    public string RefreshToken { get; set; } = default!;

    public DateTime TokenCreated { get; set; }

    public DateTime? TokenExpiration { get; set; }

    public bool IsApproved { get; set; }

    public bool IsLockedOut { get; set; }

    public int FailedLoginCount { get; set; }

    public DateTime? DateLoggedIn { get; set; }

    public DateTime? DatePwdChanged { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime? DateLockedout { get; set; }

    public User()
    {

    }

    public User(string userName, byte[] passwordHash, byte[] passwordSalt, bool isApproved)
    {
        UserName = userName;
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
        IsApproved = isApproved;
        DateCreated = DateTime.Now;
    }
}

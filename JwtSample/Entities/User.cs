using System.ComponentModel.DataAnnotations;

namespace JwtSample.Entities;
#nullable disable
public class User
{
    [Key]
    [MaxLength(20)]
    public string UserName { get; init; } = null!;

    public byte[] PasswordHash { get; set; }

    public byte[] PasswordSalt { get; set; }

    [MaxLength(256)]
    public string RefreshToken { get; set; } = null!;

    public DateTime TokenCreated { get; init; }

    public DateTime? TokenExpiration { get; set; }

    public bool IsApproved { get; set; }

    public bool IsLockedOut { get; set; }

    public int FailedLoginCount { get; set; }

    public DateTime? DateLoggedIn { get; set; }

    public DateTime? DatePwdChanged { get; init; }

    public DateTime DateCreated { get; init; }

    public DateTime? DateLockedOut { get; set; }

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

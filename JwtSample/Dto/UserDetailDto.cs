namespace JwtSample.Dto;

public record UserDetailDto
{
    public string UserName { get; set; } = default!;

    public bool IsApproved { get; set; }

    public bool IsLockedOut { get; set; }

    public int FailedLoginCount { get; set; }

    public DateTime? DateLoggedIn { get; set; }

    public DateTime? DatePwdChanged { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime? DateLockedout { get; set; }
}

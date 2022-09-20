namespace JwtSample.Auth;

public class SecuritySettings
{
    public const string ConfigSectionPath = "SecuritySettings";

    public int MaximumPasswordFailure { get; set; } = 0x3;

    public int NewUserInactiveDays { get; set; } = 0x3;

    public int RegularUserInactiveDays { get; set; } = 0xF;

    public int PasswordChangeDays { get; set; } = 0x1E;

}

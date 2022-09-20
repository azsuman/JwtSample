using System.ComponentModel.DataAnnotations;

namespace JwtSample.Auth.Jwt;

public class JwtSettings : IValidatableObject
{

    public const string ConfigSectionPath = "SecuritySettings:JwtSettings";

    public string SigningSecret { get; set; } = string.Empty;

    public int TokenExpirationInMinutes { get; set; } = 0x1;

    public int RefreshTokenExpirationInDays { get; set; } = 0x1;


    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrEmpty(SigningSecret))
        {
            yield return new ValidationResult("No value defined in Security:SecuritySettings:JwtSettings config", new[] { nameof(SigningSecret) });
        }
    }
}

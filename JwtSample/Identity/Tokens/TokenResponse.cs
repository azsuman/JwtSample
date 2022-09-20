namespace JwtSample.Identity.Tokens;

public record TokenResponse(string Token, string RefreshToken, DateTime TokenExpiryTime, DateTime RefreshTokenExpiryTime);

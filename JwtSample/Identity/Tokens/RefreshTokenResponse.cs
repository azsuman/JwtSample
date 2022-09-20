namespace JwtSample.Identity.Tokens;

public record RefreshTokenResponse(string Token, string RefreshToken, DateTime TokenExpiryTime, DateTime RefreshTokenExpiryTime);

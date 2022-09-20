namespace JwtSample.Identity.Tokens;

public record RefreshTokenRequest(string Token, string RefreshToken);

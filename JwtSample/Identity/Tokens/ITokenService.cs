using JwtSample.Interfaces;

namespace JwtSample.Identity.Tokens;

public interface ITokenService : ITransientService
{
    Task<TokenResponse> GetTokenAsync(TokenRequest request, string ipAddress);

    Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request, string ipAddress);

    Task RevokeTokenAsync(RevokeTokenRequest token);
}

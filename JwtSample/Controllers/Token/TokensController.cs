using JwtSample.Identity.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace JwtSample.Controllers.Token;

[Route("api/[controller]")]
[ApiController]
public class TokensController : ControllerBase
{
    private readonly ITokenService _tokenService;

    public TokensController(ITokenService tokenService) => _tokenService = tokenService;

    [HttpPost]
    [AllowAnonymous]
    [OpenApiOperation("Request an access token using credentials.", "Accepts POST requests containing a username and password in the body. On success, a JWT access and refresh tokens are returned.")]
    public Task<TokenResponse> GetTokenAsync(TokenRequest request)
        => _tokenService.GetTokenAsync(request, GetIpAddress());

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [OpenApiOperation("Request an access token using the refresh token.", "Accepts POST requests containing an access token with a refresh token. On success, new JWT access and refresh tokens are returned.")]
    public Task<RefreshTokenResponse> RefreshAsync(RefreshTokenRequest request)
        => _tokenService.RefreshTokenAsync(request, GetIpAddress());

    [HttpPost("revoke-token")]
    [AllowAnonymous]
    [OpenApiOperation("Request to revoke the refresh token using the access token.", "Accepts POST requests containing an access token. On success, the refresh token is revoked and no longer be used to generate new JWT access tokens.")]
    public Task RevokeAsync(RevokeTokenRequest request)
        => _tokenService.RevokeTokenAsync(request);

    private string GetIpAddress() =>
        Request.Headers.ContainsKey("X-Forwarded-For")
            ? Request.Headers["X-Forwarded-For"].ToString() ?? "N/A"
            : HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "N/A";
}

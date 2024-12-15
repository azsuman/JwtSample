using JwtSample.Auth;
using JwtSample.Auth.Jwt;
using JwtSample.Common;
using JwtSample.Data;
using JwtSample.Entities;
using JwtSample.Exceptions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace JwtSample.Identity.Tokens;

internal sealed class TokenService : ITokenService
{
    private const string IpAddress = "ipAddress";
    private readonly DataContext _dataContext;
    private readonly SecuritySettings _securitySettings;
    private readonly JwtSettings _jwtSettings;

    public TokenService(DataContext dataContext, IOptionsMonitor<SecuritySettings> securitySettings, IOptionsMonitor<JwtSettings> jwtSettings)
    {
        _dataContext = dataContext;
        _securitySettings = securitySettings.CurrentValue;
        _jwtSettings = jwtSettings.CurrentValue;
    }

    public async Task<TokenResponse> GetTokenAsync(TokenRequest request, string ipAddress)
    {
        if (await _dataContext.Users.FindAsync(request.UserName) is not { } user)
        {
            throw new InvalidUsernamePasswordException();
        }

        if (MOneCrypto.VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
        {
            _ = user switch
            {
                { IsApproved: false } => throw new InactiveAccountException(),
                { IsLockedOut: true } => throw new LockoutAccountException(),
                _ => ""
            };

            if (user.FailedLoginCount >= _securitySettings.MaximumPasswordFailure)
            {
                await LockAsync(user);
                throw new MaxPasswordFailureException();
            }

            if (user.DateLoggedIn is null && user.DateCreated.AddDays(_securitySettings.NewUserInactiveDays) < DateTime.Now)
            {
                await LockAsync(user);
                throw new NewUserInactiveDaysException(_securitySettings.NewUserInactiveDays);
            }

            if (!(user.DateLoggedIn?.AddDays(_securitySettings.RegularUserInactiveDays) < DateTime.Now))
                return await GenerateTokensAndUpdateUser(user, ipAddress);
            
            await LockAsync(user);
            throw new RegularUserInactiveDaysException(_securitySettings.RegularUserInactiveDays);
        }

        if (++user.FailedLoginCount >= _securitySettings.MaximumPasswordFailure)
        {
            user.IsLockedOut = true;
            user.DateLockedOut = DateTime.Now;
        }

        _dataContext.Users.Update(user);
        _ = await _dataContext.SaveChangesAsync();

        throw new InvalidUsernamePasswordException();
    }

    public async Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request, string ipAddress)
    {
        if (GetPrincipalFromExpiredToken(request.Token) is not { } userPrincipal)
        {
            throw new InvalidTokenException();
        }

        var userName = userPrincipal.FindFirst(ClaimTypes.Name)?.Value;

        if (await _dataContext.Users.FindAsync(userName) is not { } user)
        {
            throw new UnauthorizedException();
        }

        if (user.RefreshToken != request.RefreshToken || user.TokenExpiration <= DateTime.UtcNow)
        {
            throw new InvalidTokenException();
        }

        return await GenerateRefreshTokensAndUpdateUser(user, ipAddress);
    }

    public async Task RevokeTokenAsync(RevokeTokenRequest request)
    {
        if (GetPrincipalFromExpiredToken(request.Token) is not { } userPrincipal)
        {
            throw new InvalidTokenException();
        }

        var userName = userPrincipal.FindFirst(ClaimTypes.Name)?.Value;

        if (await _dataContext.Users.FindAsync(userName) is not { } user)
        {
            throw new UnauthorizedException();
        }

        user.RefreshToken = string.Empty;
        user.TokenExpiration = null;
        _dataContext.Update(user);
        await _dataContext.SaveChangesAsync();
    }

    private async Task<TokenResponse> GenerateTokensAndUpdateUser(User user, string ipAddress)
    {
        var (token, expires) = GenerateJwt(user, ipAddress);

        user.RefreshToken = GenerateRefreshToken();
        user.TokenExpiration = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays);

        user.DateLoggedIn = DateTime.Now;
        _dataContext.Update(user);
        await _dataContext.SaveChangesAsync();

        return new TokenResponse(token, user.RefreshToken, expires, user.TokenExpiration ?? default);
    }

    private async Task<RefreshTokenResponse> GenerateRefreshTokensAndUpdateUser(User user, string ipAddress)
    {
        var (token, expires) = GenerateJwt(user, ipAddress);

        user.RefreshToken = GenerateRefreshToken();
        user.TokenExpiration = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays);

        user.DateLoggedIn = DateTime.Now;
        _dataContext.Update(user);
        await _dataContext.SaveChangesAsync();

        return new RefreshTokenResponse(token, user.RefreshToken, expires, user.TokenExpiration ?? default);
    }

    private (string Token, DateTime Expires) GenerateJwt(User user, string ipAddress)
        => GenerateEncryptedToken(GetSigningCredentials(), GetClaims(user, ipAddress));

    private (string Token, DateTime Expires) GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            NotBefore = DateTime.UtcNow,
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.TokenExpirationInMinutes),
            SigningCredentials = signingCredentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return (tokenHandler.WriteToken(token), token.ValidTo);
    }

    private SigningCredentials GetSigningCredentials()
        => new(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SigningSecret)), SecurityAlgorithms.HmacSha512Signature);

    private static IEnumerable<Claim> GetClaims(User user, string ipAddress) =>
        new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName),
            new(IpAddress, ipAddress)
        };

    private static string GenerateRefreshToken()
    {
        byte[] randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.SigningSecret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            RequireExpirationTime = true,
            ValidateLifetime = false,
            ClockSkew = TimeSpan.Zero
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(
                    SecurityAlgorithms.HmacSha512,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                throw new UnauthorizedException();
            }

            return principal;
        }
        catch
        {
            throw new InvalidTokenException();
        }
    }

    private async Task LockAsync(User user)
    {
        user.IsLockedOut = true;
        user.DateLockedOut = DateTime.Now;
        _dataContext.Update(user);
        _ = await _dataContext.SaveChangesAsync();
    }
}
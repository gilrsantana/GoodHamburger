using GoodHamburger.Database.Accounts.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GoodHamburger.Application.IdentityServices.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;
    private readonly string _jwtKey;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;
    private readonly int _accessTokenExpiration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
        _jwtKey = _configuration["JWT:Key"] ?? throw new InvalidOperationException("JWT:Key not configured");
        _jwtIssuer = _configuration["JWT:Issuer"] ?? throw new InvalidOperationException("JWT:Issuer not configured");
        _jwtAudience = _configuration["JWT:Audience"] ?? throw new InvalidOperationException("JWT:Audience not configured");
        _accessTokenExpiration = int.Parse(_configuration["JWT:AccessTokenExpiration"] ?? "15");
    }

    public string GenerateAccessToken(ApplicationUser user, IEnumerable<string> roles, IEnumerable<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claimsList = new List<Claim>
        {
            new ("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", user.Id.ToString()),
            new ("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", user.UserName ?? ""),
            new ("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", user.Email ?? ""),
            new (JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new (JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new (JwtRegisteredClaimNames.Jti, GetJwtTokenId()),
            new (JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };
        
        foreach (var role in roles)
        {
            claimsList.Add(new Claim(ClaimTypes.Role, role));
        }

        claimsList.AddRange(claims);

        var token = new JwtSecurityToken(
            issuer: _jwtIssuer,
            audience: _jwtAudience,
            claims: claimsList,
            expires: DateTime.UtcNow.AddMinutes(_accessTokenExpiration),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey)),
            ValidateLifetime = false,
            ValidIssuer = _jwtIssuer,
            ValidAudience = _jwtAudience,
            ClockSkew = TimeSpan.Zero
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            return null;
        }

        return principal;
    }

    public string GetJwtTokenId()
    {
        return Guid.NewGuid().ToString();
    }
}

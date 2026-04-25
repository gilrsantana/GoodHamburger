using GoodHamburger.Database.Accounts.Entities;
using System.Security.Claims;

namespace GoodHamburger.Application.IdentityServices.Services;

public interface IJwtTokenService
{
    string GenerateAccessToken(ApplicationUser user, IEnumerable<string> roles, IEnumerable<Claim> claims);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    string GetJwtTokenId();
}

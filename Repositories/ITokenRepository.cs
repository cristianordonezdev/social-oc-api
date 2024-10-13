using Microsoft.AspNetCore.Identity;
using social_oc_api.Models.Domain.Auth;
using System.Security.Claims;

namespace social_oc_api.Repositories
{
    public interface ITokenRepository
    {
        string CreateJWTToken(IdentityUser user, List<string> roles, int minutes);

        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);

        Task SaveRefreshToken(string UserId, string Token);

        Task<RefreshToken?>  GetRefreshToken(string UserId, string Token);

    }
}

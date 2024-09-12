using Microsoft.AspNetCore.Identity;

namespace social_oc_api.Repositories
{
    public interface ITokenRepository
    {
        string CreateJWTToken(IdentityUser user, List<string> roles);
    }
}

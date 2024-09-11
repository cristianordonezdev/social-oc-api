using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace social_oc_api.Data
{
    public class SocialOCDBContext : IdentityDbContext<IdentityUser>
    {
        public SocialOCDBContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {   
        }
    }
}

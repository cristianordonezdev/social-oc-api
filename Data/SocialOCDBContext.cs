using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using social_oc_api.Models.Domain;

namespace social_oc_api.Data
{
    public class SocialOCDBContext : IdentityDbContext<ApplicationUser>
    {
        public SocialOCDBContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {   
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            base.OnModelCreating(modelBuilder);

            //seed data for roles and users
            var readerRoleId = "d5223cbc-9bdc-4088-ae3b-345e281c571b";
            var writerRoleId = "3b2d71d6-cb9e-419f-a805-5be22fc395fe";

            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = readerRoleId,
                    ConcurrencyStamp = readerRoleId,
                    Name = "Reader",
                    NormalizedName = "Reader".ToUpper()
                },
                new IdentityRole
                {
                    Id = writerRoleId,
                    ConcurrencyStamp = writerRoleId,
                    Name = "Writer",
                    NormalizedName = "Writer".ToUpper()
                }
            }; ;

            modelBuilder.Entity<IdentityRole>().HasData(roles);
        }
}
}

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using social_oc_api.Models.Domain;
using social_oc_api.Models.Domain.Auth;
using social_oc_api.Models.Domain.Images;

namespace social_oc_api.Data
{
    public class SocialOCDBContext : IdentityDbContext<ApplicationUser>
    {
        public SocialOCDBContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
        }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostImage> PostImages { get; set; }
        public DbSet<UserImage> UserImages { get; set; }
        public DbSet<Follower> Followers { get; set; }
        public DbSet<RequestFollower> RequestFollowers { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(u => u.UserName)
                .IsUnique();
            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(u => u.Email)
                .IsUnique();
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Posts)
                .WithOne(ui => ui.User)
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.ImageProfile)
                .WithOne(ui => ui.User)
                .HasForeignKey<UserImage>(ui => ui.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Follower>(entity =>
            {
                // Relación para el Follower (quién sigue)
                entity.HasOne(f => f.FollowerUser)
                      .WithMany(u => u.Followers) // Relación de "seguidores"
                      .HasForeignKey(f => f.FollowerId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Relación para el Following (a quién sigue)
                entity.HasOne(f => f.FollowingUser)
                      .WithMany() // No necesitas otra colección aquí
                      .HasForeignKey(f => f.FollowingId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<RequestFollower>()
                .HasOne(u => u.FollowerUser)
                .WithMany()
                .HasForeignKey(ui => ui.FollowingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Post>()
                .HasMany(p => p.PostImages)
                .WithOne(pi => pi.Post)
                .HasForeignKey(pi => pi.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Post>()
                .HasMany(p => p.Likes)
                .WithOne(pi => pi.Post)
                .HasForeignKey(pi => pi.PostId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Post>()
                .HasMany(p => p.Comments)
                .WithOne(pi => pi.Post)
                .HasForeignKey(pi => pi.PostId)
                .OnDelete(DeleteBehavior.Restrict);


            // Seed data for roles and users
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
            };
            modelBuilder.Entity<IdentityRole>().HasData(roles);
        }
    }
}

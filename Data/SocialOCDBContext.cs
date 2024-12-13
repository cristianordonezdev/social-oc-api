using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using social_oc_api.Models.Domain;
using social_oc_api.Models.Domain.Auth;
using social_oc_api.Models.Domain.Chat;
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
        public DbSet<Message> Messages { get; set; }
        public DbSet<Conversation> Conversations { get; set; }

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
                // relation for follower (who follows)
                entity.HasOne(f => f.FollowerUser)
                      .WithMany(u => u.Followers) // relation of "followers"
                      .HasForeignKey(f => f.FollowerId)
                      .OnDelete(DeleteBehavior.Restrict);

                // relation for following (who following)
                entity.HasOne(f => f.FollowingUser)
                      .WithMany()
                      .HasForeignKey(f => f.FollowingId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<RequestFollower>()
                .HasOne(u => u.FollowerUser)
                .WithMany()
                .HasForeignKey(ui => ui.FollowerId)
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

            // Relationship between conversation and message
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Conversation)
                .WithMany()
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            // relationship between conversation and user
            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.ParticipantOne)
                .WithMany()
                .HasForeignKey(c => c.ParticipantOneId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.ParticipantSecond)
                .WithMany()
                .HasForeignKey(c => c.ParticipantSecondId)
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

﻿using Microsoft.AspNetCore.Identity;
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

            modelBuilder.Entity<Post>()
                    .HasMany(p => p.PostImages)
                    .WithOne(pi => pi.Post)
                    .HasForeignKey(pi => pi.PostId)
                    .OnDelete(DeleteBehavior.Cascade);

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

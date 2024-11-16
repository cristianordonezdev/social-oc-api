﻿using Microsoft.EntityFrameworkCore;
using social_oc_api.Data;
using social_oc_api.Models.Domain;
using social_oc_api.Models.Domain.Images;
using social_oc_api.Models.DTO.Posts;

using social_oc_api.Models.DTO.User;
using social_oc_api.Utils;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace social_oc_api.Repositories
{
    public class SQLPostRepository : IPostRepository
    {
        private readonly SocialOCDBContext _dbContext;
        private readonly IImageRepository _imageRepository;
        private readonly IUtils _utils;
        public SQLPostRepository(SocialOCDBContext dbContext, IImageRepository imageRepository, IUtils utils)
        {
            _dbContext = dbContext;
            _imageRepository = imageRepository;
            _utils = utils;
        }
        public async Task<Post> CreatePost(Post post, List<IFormFile> files)
        {
            await _dbContext.Posts.AddAsync(post);
            await _dbContext.SaveChangesAsync();

            foreach (var file in files)
            {
                var imageDomain = new PostImage
                {
                    File = file,
                    PostId = post.Id,
                };

               var imageUploadedDomain = await _imageRepository.UploadImage(imageDomain, "PostImages");
            }
            return post;
        }

        public async Task<Post?> deletePost(Guid postId, Guid OwnUserId)
        {
            var postDomain = await _dbContext.Posts.Include(post => post.PostImages).FirstOrDefaultAsync(post => post.Id == postId && post.UserId == OwnUserId.ToString());
            if (postDomain == null) { return null; }

            foreach (var image in postDomain.PostImages)
            {
                _utils.DeleteImageFromFolder(image.FilePath);
            }
            _dbContext.Posts.Remove(postDomain);
            await _dbContext.SaveChangesAsync();

            return postDomain;
        }

        // Post of Home, of followers
        public async Task<List<Post>> GetPostsHome(string ownUserId)
        {
            var followersIds = await _dbContext.Followers
               .Where(f => f.FollowerId.ToString() == ownUserId)
               .Select(f => f.FollowingId.ToString())
               .ToListAsync();

            followersIds.Add(ownUserId);

            var postsWithImages = await _dbContext.Posts
                .Where(post => followersIds.Contains(post.UserId))
                .Include(post => post.PostImages)
                .ToListAsync();

            return postsWithImages;
        }

        public async Task<List<Models.DTO.Posts.PostProfileDto>> GetPostsOf(Guid userId, int page, int pageSize)
        {

            int skip = (page - 1) * pageSize;

            var postProfiles = await _dbContext.Posts
                .Where(post => post.UserId == userId.ToString())
                .OrderByDescending(post => post.CreatedAt)
                .Skip(skip)
                .Take(pageSize)
                .Select(post => new Models.DTO.Posts.PostProfileDto
                {
                    Id = post.Id,
                    Image = post.PostImages.FirstOrDefault().FilePath ?? "",
                    LikesCount = _dbContext.Likes.Count(like => like.PostId == post.Id),
                    CommentsCount = _dbContext.Comments.Count(comment => comment.PostId == post.Id)
                })
                .ToListAsync();

            return postProfiles;
        }
        public async Task<PostDetailDto?> GetPostDetail(Guid postId)
        {
            var postDomain = await _dbContext.Posts
            .Where(i => i.Id == postId)
            .Select(post => new PostDetailDto
                {
                    Id = post.Id,
                    Caption = post.Caption,
                    CreatedAt = post.CreatedAt,
                    UpdatedAt = post.UpdatedAt,
                    PostImages = post.PostImages.Select(img => new ImageDto
                    {
                        Id = img.Id,
                        FilePath = img.FilePath
                    }).ToList(),
                    User = new UserDetailDto
                    {
                        UserName = post.User.UserName,
                        ImageProfile = new ImageDto
                        {
                            Id = post.User.ImageProfile.Id,
                            FilePath = post.User.ImageProfile.FilePath
                        }
                    },
                    Likes = null,
                    Comments = null
                })
                .FirstOrDefaultAsync();
            return postDomain;
        }

        public async Task<Like?> LikePost(string userId, Guid postId)
        {
            var likeDomainSaved = await _dbContext.Likes
                .FirstOrDefaultAsync(i => i.PostId == postId && i.UserId == userId);

            if (likeDomainSaved == null)
            {
                var postExists = await _dbContext.Posts.AnyAsync(i => i.Id == postId);
                if (!postExists) return null;

                var likeDomain = new Like
                {
                    UserId = userId,
                    PostId = postId,
                };

                await _dbContext.Likes.AddAsync(likeDomain);
                await _dbContext.SaveChangesAsync();
                return likeDomain;
            }

            _dbContext.Likes.Remove(likeDomainSaved);
            await _dbContext.SaveChangesAsync();

            return likeDomainSaved;
        }

        public async Task<CommentFullDto?> CommentPost(string userId, Guid postId, string comment)
        {
            var postExists = await _dbContext.Posts.AnyAsync(i => i.Id == postId);
            if (!postExists) return null;

            Comment commentDomain = new Comment
            {
                UserId = userId,
                PostId = postId,
                CommentText = comment
            };

            await _dbContext.Comments.AddAsync(commentDomain);
            await _dbContext.SaveChangesAsync();

            await _dbContext.Entry(commentDomain)
                   .Reference(c => c.User)
                   .LoadAsync();
            await _dbContext.Entry(commentDomain.User)
                       .Reference(u => u.ImageProfile)
                       .LoadAsync();

            var commentDto = new CommentFullDto
            {
                Id = commentDomain.Id,
                CommentText = commentDomain.CommentText,
                CreatedAt = commentDomain.CreatedAt,
                username = commentDomain.User.UserName,
                imageProfile = commentDomain.User.ImageProfile?.FilePath,
            };


            return commentDto;
        }


    }
}

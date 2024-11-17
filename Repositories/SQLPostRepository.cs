using Microsoft.EntityFrameworkCore;
using social_oc_api.Data;
using social_oc_api.Models.Domain;
using social_oc_api.Models.Domain.Images;
using social_oc_api.Models.DTO.Posts;
using social_oc_api.Models.DTO.User;
using social_oc_api.Utils;
using System.Reflection.Metadata;
using System.Xml.Linq;

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

            var comments = _dbContext.Comments.Where(c => c.PostId == postId);
            _dbContext.Comments.RemoveRange(comments);

            var likes = _dbContext.Likes.Where(l => l.PostId == postId);
            _dbContext.Likes.RemoveRange(likes);


            foreach (var image in postDomain.PostImages)
            {
                _utils.DeleteImageFromFolder(image.FilePath);
            }
            _dbContext.Posts.Remove(postDomain);
            await _dbContext.SaveChangesAsync();

            return postDomain;
        }

        // Post of Home, of followers
        public async Task<List<Models.DTO.Posts.PostHomeDto>> GetPostsHome(string ownUserId, int page, int pageSize)
        {
            int skip = (page - 1) * pageSize;
            var followersIds = await _dbContext.Followers
                .Where(f => f.FollowerId.ToString() == ownUserId)
                .Select(f => f.FollowingId.ToString())
                .ToListAsync();

            followersIds.Add(ownUserId);

            var postsWithImages = await _dbContext.Posts
                .Where(post => followersIds.Contains(post.UserId))
                .Include(post => post.PostImages)
                .Include(post => post.User)
                    .ThenInclude(user => user.ImageProfile)
                .Select(post => new Models.DTO.Posts.PostHomeDto
                {
                    Id = post.Id,
                    Caption = post.Caption,
                    CreatedAt = post.CreatedAt,
                    Images = post.PostImages.Select(image => new ImageDto
                    {
                        Id = image.Id,
                        FilePath = image.FilePath,
                    }).ToList(),
                    User = new Models.DTO.Posts.PostHomeUser
                    {
                        ImageProfile = post.User.ImageProfile.FilePath,
                        Username = post.User.UserName,
                    },
                    CommentsCount = post.Comments.Count,
                    FirstLikeUsername = post.Likes.OrderBy(like => like.CreatedAt)
                                                  .Select(like => like.User.UserName)
                                                  .FirstOrDefault(),
                    HasMoreLikes = post.Likes.Count > 1
                })
                .OrderByDescending(post => post.CreatedAt)
                .Skip(skip)
                .Take(pageSize)
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

        public async Task<List<UserListDto>?> LikesUsers(Guid? PostId, string OwnUserId, int page, int pageSize)
        {
            int skip = (page - 1) * pageSize;

            return await _dbContext.Likes
            .Where(like => like.PostId == PostId)
            .Select(like => new UserListDto
            {
                Username = like.User.UserName,
                Name = like.User.Name,
                ImageProfile = like.User.ImageProfile.FilePath,
                AreYouFollowing = _dbContext.Followers
                    .Any(f => f.FollowerId == OwnUserId && f.FollowingId == like.User.Id),
            })
            .OrderByDescending(like => like.AreYouFollowing)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();
            }

        public async Task<List<CommentFullDto>?> GetCommentsPosts(Guid postId, int page, int pageSize)
        {
            int skip = (page - 1) * pageSize;
            var commentsDomain = await _dbContext.Comments.Where(i => i.PostId == postId)
                .Include(post => post.User)
                .OrderByDescending(comment => comment.CreatedAt)
                .Skip(skip)
                .Take(pageSize)
                .Select(post => new CommentFullDto
                {
                    Id = post.Id,
                    CommentText = post.CommentText,
                    CreatedAt = post.CreatedAt,
                    username = post.User.UserName,
                    imageProfile = post.User.ImageProfile.FilePath,
                })
                .ToListAsync();

            return commentsDomain;
        }

        public async Task<Boolean?> deleteComment(Guid commentId, string OwnUserId)
        {
            var commentDomain = await _dbContext.Comments
                .Include(comment => comment.Post)
                    .ThenInclude(post => post.User)
                .FirstOrDefaultAsync(i => i.Id == commentId);

            if ((commentDomain?.Post.User.Id == OwnUserId) || commentDomain?.UserId == OwnUserId)
            {
                _dbContext.Comments.Remove(commentDomain);
                await _dbContext.SaveChangesAsync();
                return true;
            }

            return null;
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
                    ImageProfile = post.User.ImageProfile.FilePath,

                },
                FirstLikeUsername = post.Likes.OrderBy(like => like.CreatedAt)
                                   .Select(like => like.User.UserName)
                                   .FirstOrDefault(),
                HasMoreLikes = post.Likes.Count > 1
            })
                .FirstOrDefaultAsync();
            return postDomain;
        }
        public async Task<PostDetailDto?> UpdatePost(string captionUpdated, Guid postId, string ownUserId)
        {
            var postDomain = await _dbContext.Posts.FirstOrDefaultAsync(i => (i.Id == postId) && (i.UserId == ownUserId));
            if (postDomain == null) { return null; }

            postDomain.Caption = captionUpdated;
            postDomain.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            var postDomainUpdated = await GetPostDetail(postDomain.Id);
            return postDomainUpdated;
        }
        public async Task<Boolean?> deleteImagePost(Guid imageId, string OwnUserId)
        {
            var imageDomain = await _dbContext.PostImages.Include(i => i.Post).ThenInclude(post => post.PostImages).FirstOrDefaultAsync(i => i.Id == imageId && i.Post.UserId == OwnUserId);
            if (imageDomain == null) { return null; }

            if (imageDomain.Post.PostImages.Count == 1)
            {
                return false;
            } else
            {
                foreach (var image in imageDomain.Post.PostImages)
                {
                    _utils.DeleteImageFromFolder(image.FilePath);
                }
                _dbContext.Remove(imageDomain);
                await _dbContext.SaveChangesAsync();
          
                return true;
            }

        }
    }
}

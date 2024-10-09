using Microsoft.EntityFrameworkCore;
using social_oc_api.Data;
using social_oc_api.Models.Domain;
using social_oc_api.Models.Domain.Images;
using social_oc_api.Utils;

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
            var postDomain = await _dbContext.Posts.Include(post => post.PostImages).FirstOrDefaultAsync(post => post.Id == postId && post.UserId == OwnUserId);
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
        public async Task<List<Post>> GetPostsHome(Guid ownUserId)
        {
            var followersIds = await _dbContext.Followers
               .Where(f => f.FollowerId == ownUserId)
               .Select(f => f.FollowingId)
               .ToListAsync();

            followersIds.Add(ownUserId);

            var postsWithImages = await _dbContext.Posts
                .Where(post => followersIds.Contains(post.UserId))
                .Include(post => post.PostImages)
                .ToListAsync();

            return postsWithImages;
        }

        public async Task<List<Post>> GetPostsOf(Guid userId)
        {
            var postsWithImages = await _dbContext.Posts
                 .Where(post => post.UserId == userId)
                 .Include(post => post.PostImages)
                 .ToListAsync();

            return postsWithImages;
        }
    }
}

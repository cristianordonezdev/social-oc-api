using Microsoft.EntityFrameworkCore;
using social_oc_api.Data;
using social_oc_api.Models.Domain;

namespace social_oc_api.Repositories
{
    public class SQLPostRepository : IPostRepository
    {
        private readonly SocialOCDBContext _dbContext;
        private readonly IImageRepository _imageRepository;
        public SQLPostRepository(SocialOCDBContext dbContext, IImageRepository imageRepository)
        {
            _dbContext = dbContext;
            _imageRepository = imageRepository;
        }
        public async Task<Post> CreatePost(Post post, List<IFormFile> files)
        {
            await _dbContext.Posts.AddAsync(post);
            await _dbContext.SaveChangesAsync();

            foreach (var file in files)
            {
                var imageDomain = new Image
                {
                    File = file,
                    ReferenceId = post.Id,
                };

                var imageUploadedDomain = await _imageRepository.UploadImage(imageDomain);
                if (post.Files == null)
                {
                    post.Files = new List<Image> { imageUploadedDomain };
                }
                else
                {
                    post.Files.Add(imageUploadedDomain);
                }
            }
            return post;
        }

        // Post of Home, of followers
        public async Task<List<Post>> GetPostsHome()
        {
            var posts = await _dbContext.Posts.ToListAsync();
            return posts;
        }

        public async Task<List<Post>> GetPostsOf(Guid userId)
        {
            var posts = await _dbContext.Posts.Where(post => post.UserId == userId).ToListAsync();
            return posts;   
        }
    }
}

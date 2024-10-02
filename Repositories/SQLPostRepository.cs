using Microsoft.EntityFrameworkCore;
using social_oc_api.Data;
using social_oc_api.Models.Domain;

namespace social_oc_api.Repositories
{
    public class SQLPostRepository : IPostRepository
    {
        private readonly SocialOCDBContext _dbContext;
        public SQLPostRepository(SocialOCDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Post> CreatePost(Post post)
        {
            await _dbContext.Posts.AddAsync(post);
            await _dbContext.SaveChangesAsync();
            return post;
        }

        // Post of Home, of followers
        public async Task<List<Post>> GetPostsHome()
        {
            var posts = await _dbContext.Posts.ToListAsync();
            return posts;
        }



    }
}

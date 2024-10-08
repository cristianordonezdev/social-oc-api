using Microsoft.AspNetCore.Identity;
using social_oc_api.Models.Domain;
using social_oc_api.Models.Domain.Auth;
using System.Security.Claims;

namespace social_oc_api.Repositories
{
    public interface IPostRepository
    {
        Task<Post> CreatePost(Post post, List<IFormFile> files);
        Task<List<Post>> GetPostsHome(Guid ownUserId);

        Task<List<Post>> GetPostsOf(Guid userId);

        Task<Post?> deletePost(Guid postId, Guid OwnUserId);
    }
}

using Microsoft.AspNetCore.Identity;
using social_oc_api.Models.Domain;
using social_oc_api.Models.Domain.Auth;
using System.Security.Claims;

namespace social_oc_api.Repositories
{
    public interface IPostRepository
    {
        Task<Post> CreatePost(Post post);
        Task<List<Post>> GetPostsHome();

    }
}

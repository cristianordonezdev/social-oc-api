using Microsoft.AspNetCore.Identity;
using social_oc_api.Models.Domain;
using social_oc_api.Models.Domain.Auth;
using social_oc_api.Models.DTO.Posts;
using social_oc_api.Models.DTO.User;
using System.Security.Claims;

namespace social_oc_api.Repositories
{
    public interface IPostRepository
    {
        Task<Post> CreatePost(Post post, List<IFormFile> files);

        Task<PostDetailDto?> UpdatePost(string captionUpdated, Guid postId, string ownUserId);

        Task<List<PostHomeDto>> GetPostsHome(string ownUserId, int page, int pageSize);

        Task<List<UserListDto>?> LikesUsers(Guid? PostId, string OwnUserId, int page, int pageSize);

        Task<List<PostProfileDto>?> GetPostsOf(string userId, int page, int pageSize);

        Task<ResponsePostDetailDto?> GetPostDetail(Guid postId, string ownUserId);

        Task<Like?> LikePost(string userId, Guid postId);

        Task<CommentFullDto?> CommentPost(string userId, Guid postId, string comment);

        Task<List<CommentFullDto>?> GetCommentsPosts(Guid postId, int page, int pageSize);

        Task<Boolean?> deleteComment(Guid commentId, string OwnUserId);

        Task<Post?> deletePost(Guid postId, Guid OwnUserId);

        Task<Boolean?> deleteImagePost(Guid imageId, string OwnUserId);

    }
}

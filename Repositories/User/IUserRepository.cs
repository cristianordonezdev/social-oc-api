using social_oc_api.Models.Domain;
using social_oc_api.Models.Domain.Images;
using social_oc_api.Models.DTO.User;

namespace social_oc_api.Repositories.User
{
    public interface IUserRepository
    {
        Task<bool> DeleteUser(Guid userId);

        Task<ApplicationUser> UploadImageProfile(UserImage userImage);

        Task<ProfileUser?> GetProfile(string username);

        Task<List<UserListDto>?> GetFollowerOrFollowing(string userId, string followAction, string OwnUserId, int page, int pageSize);

    }
}

using social_oc_api.Models.Domain;
using social_oc_api.Models.Domain.Images;

namespace social_oc_api.Repositories.User
{
    public interface IUserRepository
    {
        Task<bool> DeleteUser(Guid userId);

        Task<ApplicationUser> UploadImageProfile(UserImage userImage);
    }
}

using social_oc_api.Models.Domain;
using social_oc_api.Models.DTO.Followers;

namespace social_oc_api.Repositories
{
    public interface IFollowerRepository
    {
        Task<Follower?> ToggleFollowAction(Follower follower);

        Task<Boolean> GetVisibility(string userId, string ownUserId);

        Task<bool> HandleGetRequest(Follower follower);

        Task<List<RequestFollowerDto>> GetListOfRequests(string userId, int page, int pageSize);

        Task<bool?> deleteRequest(Guid requestId, string userId);

        Task<Follower?> acceptRequest(Guid requestId, string userId);
    }
}

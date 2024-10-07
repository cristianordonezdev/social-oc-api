using social_oc_api.Models.Domain;

namespace social_oc_api.Repositories
{
    public interface IFollowerRepository
    {
        Task<Follower?> ToggleFollowAction(Follower follower);
    }
}

namespace social_oc_api.Models.Domain
{
    public class RequestFollower
    {
        public Guid Id { get; set; }

        public string FollowerId { get; set; }

        public string FollowingId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ApplicationUser FollowerUser { get; set; }

    }
}

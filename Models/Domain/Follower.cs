namespace social_oc_api.Models.Domain
{
    public class Follower
    {
        public Guid Id { get; set; }

        public Guid FollowerId { get; set; }

        public Guid FollowingId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}

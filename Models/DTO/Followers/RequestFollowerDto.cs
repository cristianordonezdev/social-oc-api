using social_oc_api.Models.DTO.Posts;

namespace social_oc_api.Models.DTO.Followers
{
    public class RequestFollowerDto
    {
        public Guid Id { get; set; }

        public string FollowerId { get; set; }

        public string FollowingId { get; set; }

        public DateTime CreatedAt { get; set; }

        public PostHomeUser User { get; set; }
    }
}

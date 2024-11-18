namespace social_oc_api.Models.DTO.Followers
{
    public class FollowerDto
    {
        public Guid Id { get; set; }

        public string FollowerId { get; set; }

        public string FollowingId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}

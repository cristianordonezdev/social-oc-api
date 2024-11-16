namespace social_oc_api.Models.DTO.Posts
{
    public class LikePostDto
    {
        public string UserId { get; set; }

        public Guid PostId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}

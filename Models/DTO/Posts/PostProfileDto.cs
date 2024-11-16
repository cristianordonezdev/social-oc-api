using social_oc_api.Models.Domain;

namespace social_oc_api.Models.DTO.Posts
{
    public class PostProfileDto
    {
        public Guid Id { get; set; }

        public string Image { get; set; }

        public int LikesCount { get; set; }

        public int CommentsCount { get; set; }
    }
}

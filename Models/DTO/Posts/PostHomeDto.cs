using social_oc_api.Models.Domain.Images;
using social_oc_api.Models.Domain;

namespace social_oc_api.Models.DTO.Posts
{
    public class PostHomeDto
    {
        public Guid Id { get; set; }

        public string? Caption { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<ImageDto> Images { get; set; }

        public PostHomeUser User { get; set; }

        public string FirstLikeUsername { get; set; }

        public int CommentsCount { get; set; }

        public Boolean HasMoreLikes { get; set; }
    }

    public class PostHomeUser
    {
        public string Username { get; set; }

        public string ImageProfile { get; set; }

    }
}

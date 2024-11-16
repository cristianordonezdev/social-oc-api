using social_oc_api.Models.Domain.Auth;
using social_oc_api.Models.Domain.Images;

namespace social_oc_api.Models.Domain
{
    public class Post
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }

        public string? Caption { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public ICollection<PostImage> PostImages { get; set; }

        public ApplicationUser User { get; set; }

        public ICollection<Like> Likes { get; set; }

        public ICollection<Comment> Comments { get; set; }

    }
}

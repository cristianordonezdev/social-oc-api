using social_oc_api.Models.Domain.Auth;
using social_oc_api.Models.Domain.Images;

namespace social_oc_api.Models.Domain
{
    public class Post
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public string? Caption { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public ICollection<PostImage> PostImages { get; set; }
    }
}

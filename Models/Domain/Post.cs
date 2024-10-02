using static System.Net.Mime.MediaTypeNames;
using System.ComponentModel.DataAnnotations.Schema;

namespace social_oc_api.Models.Domain
{
    public class Post
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public string? Caption { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }


        [NotMapped]
        public required List<Image> Files { get; set; } = new List<Image>();
    }
}

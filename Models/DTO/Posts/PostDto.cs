using System.ComponentModel.DataAnnotations.Schema;

namespace social_oc_api.Models.DTO.Posts
{
    public class PostDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public string Caption { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

/*        public List<> Files { get; set; }
*/    }
}

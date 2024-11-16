using System.ComponentModel.DataAnnotations;

namespace social_oc_api.Models.DTO.Posts
{
    public class CommentDto
    {
        [MaxLength(1000)]
        public string CommentText { get; set; }

    }
}

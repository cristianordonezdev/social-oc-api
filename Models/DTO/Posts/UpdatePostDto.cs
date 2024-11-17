using System.ComponentModel.DataAnnotations;

namespace social_oc_api.Models.DTO.Posts
{
    public class UpdatePostDto
    {
        [MaxLength(255)]
        public string? Caption { get; set; }
    }
}

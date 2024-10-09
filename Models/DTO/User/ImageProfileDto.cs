using System.ComponentModel.DataAnnotations;

namespace social_oc_api.Models.DTO.User
{
    public class ImageProfileDto
    {
        [Required]
        public required IFormFile File { get; set; }

    }
}

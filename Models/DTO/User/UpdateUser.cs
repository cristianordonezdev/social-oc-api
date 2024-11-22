using System.ComponentModel.DataAnnotations;

namespace social_oc_api.Models.DTO.User
{
    public class UpdateUser
    {
        [Required]
        [MaxLength(255)]

        public string Name { get; set; }

        [MaxLength(1000)]
        [Required]

        public string Description { get; set; }

        [Required]
        public bool IsPublic { get; set; }
    }
}

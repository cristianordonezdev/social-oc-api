using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace social_oc_api.Models.DTO.Posts
{
    public class PostCreateDto : IValidatableObject
    {
        [MaxLength(255)]
        public string? Caption { get; set; }


        [Required]
        public required List<IFormFile> Files { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Files != null && Files.Count > 5)
            {
                yield return new ValidationResult("You can not upload more of 3 files.", new[] { nameof(Files) });
            }
        }
    }
}

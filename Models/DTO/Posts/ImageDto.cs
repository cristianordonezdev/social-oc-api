using System.ComponentModel.DataAnnotations.Schema;

namespace social_oc_api.Models.DTO.Posts
{
    public class ImageDto
    {
        public Guid Id { get; set; }

        public string FilePath { get; set; }
    }
}

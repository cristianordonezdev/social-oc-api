using System.ComponentModel.DataAnnotations.Schema;

namespace social_oc_api.Models.Domain
{
    public class Image
    {
        public Guid Id { get; set; }

        public Guid ReferenceId { get; set; }

        [NotMapped]
        public IFormFile File { get; set; }

        public string FilePath { get; set; }

    }
}

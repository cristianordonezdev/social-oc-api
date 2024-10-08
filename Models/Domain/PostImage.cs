using System.ComponentModel.DataAnnotations.Schema;

namespace social_oc_api.Models.Domain
{
    public class PostImage : Image
    {
        public Guid PostId { get; set; }

        public Post Post { get; set; }
        
    }
}

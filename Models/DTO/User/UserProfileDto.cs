using social_oc_api.Models.Domain;
using social_oc_api.Models.DTO.Posts;

namespace social_oc_api.Models.DTO.User
{
    public class UserProfileDto
    {
        public string Name { get; set; }
        public string UserName { get; set; }
        public string? ImageProfile { get; set; }
        public string Description { get; set; }
        public MetricsProfile MetricsProfile { get; set; }
    }
}

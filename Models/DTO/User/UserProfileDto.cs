using social_oc_api.Models.Domain;
using social_oc_api.Models.DTO.Posts;

namespace social_oc_api.Models.DTO.User
{
    public class UserProfileDto
    {
        public string Name { get; set; }
        public string UserName { get; set; }
        public string? ImageProfile { get; set; }
        public List<PostProfileDto> Posts { get; set; }
        public MetricsProfile MetricsProfile { get; set; }
    }

    public class PostProfileDto
    {
        public Guid Id { get; set; }
        public string FilePath { get; set; }

    }

}

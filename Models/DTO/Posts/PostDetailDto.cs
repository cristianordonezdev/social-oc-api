namespace social_oc_api.Models.DTO.Posts
{
    public class PostDetailDto
    {
        public Guid Id { get; set; }
        public string Caption { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<ImageDto> PostImages { get; set; }
        public UserDetailDto User { get; set; }
        public string FirstLikeUsername { get; set; }
        public Boolean HasMoreLikes { get; set; }

    }

    public class UserDetailDto
    {
        public string UserName { get; set; }
        public string ImageProfile { get; set; }
    }
}

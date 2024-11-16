namespace social_oc_api.Models.DTO.Posts
{
    public class CommentFullDto
    {
        public Guid Id { get; set; }

        public string CommentText { get; set; }

        public DateTime CreatedAt { get; set; }

        public string username { get; set; }

        public string imageProfile { get; set; }
    }
}

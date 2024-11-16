﻿namespace social_oc_api.Models.Domain
{
    public class Comment
    {
        public Guid Id { get; set; }

        public string UserId { get; set; }

        public Guid PostId { get; set; }
        public string CommentText { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ApplicationUser User { get; set; }

        public Post Post { get; set; }
    }
}
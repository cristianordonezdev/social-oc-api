﻿using Microsoft.AspNetCore.Identity;
using social_oc_api.Models.Domain.Images;

namespace social_oc_api.Models.Domain
{
    public class ApplicationUser: IdentityUser
    {
        public string Name { get; set; }

        public string Description { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public virtual UserImage ImageProfile { get; set; }
        public ICollection<Post> Posts { get; set; }

        public ICollection<Like> Likes { get; set; }

    }
}

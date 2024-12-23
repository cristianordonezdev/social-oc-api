﻿using social_oc_api.Models.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace social_oc_api.Models.DTO.Posts
{
    public class PostDto
    {
        public Guid Id { get; set; }
        public ApplicationUser User { get; set; }

        public string Caption { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public List<ImageDto> Images { get; set; }
    }
}

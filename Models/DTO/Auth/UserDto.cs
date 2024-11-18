namespace social_oc_api.Models.DTO.Auth
{
    public class UserDto
    {
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }

        public string ImageProfile { get; set; }

        public bool IsPublic { get; set; }
    }
}

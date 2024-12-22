namespace social_oc_api.Models.DTO.Chats
{
    public class MessageDto
    {
        public Guid Id { get; set; }

        public string SenderId { get; set; }

        public string MessageText { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;
    }
}

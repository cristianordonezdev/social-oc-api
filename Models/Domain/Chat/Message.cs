namespace social_oc_api.Models.Domain.Chat
{
    public class Message
    {
        public Guid Id { get; set; }

        public Guid ConversationId { get; set; }

        public string SenderId { get; set; }

        public string MessageText { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsRead {  get; set; }

        public ApplicationUser User { get; set; }

        public Conversation Conversation { get; set; }
    }
}

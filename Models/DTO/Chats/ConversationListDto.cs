namespace social_oc_api.Models.DTO.Chats
{
    public class ConversationListDto
    {
        public Guid Id { get; set; }

        public string TokenConversation {  get; set; }

        public ConversationParticipantSecond ParticipantSecond { get; set; }

        public MessageList MessageList { get; set; }

    }

    public class ConversationParticipantSecond
    {
        public string Username { get; set; }

        public string Name { get; set; }

        public string ImageProfile { get; set; }

    }

    public class MessageList
    {
        public string LastMessage { get; set; }

        public string ParticipantId { get; set; }

        public DateTime LastMessageTime { get; set; }

        public bool IsRead { get; set; }

    }
}

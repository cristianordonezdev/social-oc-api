namespace social_oc_api.Models.DTO.Chats
{
    public class ConversationDto
    {
        public Guid Id { get; set; }

        public string ParticipantOneId { get; set; }

        public string ParticipantSecondId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}

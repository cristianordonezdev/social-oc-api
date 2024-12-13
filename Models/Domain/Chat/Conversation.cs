namespace social_oc_api.Models.Domain.Chat
{
    public class Conversation
    {
        public Guid Id { get; set; }

        public string ParticipantOneId { get; set; }

        public string ParticipantSecondId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ApplicationUser ParticipantOne { get; set; }

        public ApplicationUser ParticipantSecond { get; set; }

    }
}

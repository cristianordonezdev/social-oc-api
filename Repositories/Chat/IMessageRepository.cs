using social_oc_api.Models.Domain.Chat;

namespace social_oc_api.Repositories.Chat
{
    public interface IMessageRepository
    {
        Task<Message> SendMessage(Message message);

        Task <List<Message>> getMessagesConversation(Guid conversationId, int page, int pageSize);

    }
}

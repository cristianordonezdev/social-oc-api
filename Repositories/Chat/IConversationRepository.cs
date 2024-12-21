using social_oc_api.Models.Domain;
using social_oc_api.Models.Domain.Chat;
using social_oc_api.Models.DTO.Chats;

namespace social_oc_api.Repositories.Chat
{
    public interface IConversationRepository
    {
        Task<bool> GetConversation(string userOneId, string userSecondId);

        Task<Conversation> CreateConversation(Conversation conversation);

        Task<List<ConversationListDto>> GetConversations(string userOneId, int page, int pageSize);

    }
}

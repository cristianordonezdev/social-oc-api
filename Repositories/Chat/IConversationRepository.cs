using social_oc_api.Models.Domain;
using social_oc_api.Models.Domain.Chat;

namespace social_oc_api.Repositories.Chat
{
    public interface IConversationRepository
    {
        Task<bool> GetConversation(string userOneId, string userSecondId);

        Task<Conversation> CreateConversation(Conversation conversation);

        Task<List<Conversation>> GetConversations(string userOneId);

    }
}

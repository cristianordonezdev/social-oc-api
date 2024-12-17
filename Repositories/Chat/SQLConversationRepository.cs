using Microsoft.EntityFrameworkCore;
using social_oc_api.Data;
using social_oc_api.Models.Domain.Chat;

namespace social_oc_api.Repositories.Chat
{
    public class SQLConversationRepository : IConversationRepository
    {
        private readonly SocialOCDBContext _dbContext;

        public SQLConversationRepository(SocialOCDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Conversation> CreateConversation(Conversation conversation)
        {
            var conversationDomain = await _dbContext.Conversations.AddAsync(conversation);
            await _dbContext.SaveChangesAsync();
            return conversation;
        }

        public async Task<bool> GetConversation(string userOneId, string userSecondId)
        {
            var conversationExists = await _dbContext.Conversations.AnyAsync(i => i.ParticipantOneId == userOneId && i.ParticipantSecondId == userSecondId);
            return conversationExists;
        }

        public async Task<List<Conversation>> GetConversations(string userOneId)
        {
            var conversations = await _dbContext.Conversations.Include(c => c.ParticipantSecond).Where(i => i.ParticipantOneId == userOneId).ToListAsync();
            return conversations;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using social_oc_api.Data;
using social_oc_api.Models.Domain.Chat;

namespace social_oc_api.Repositories.Chat
{
    public class SQLMessageRepository : IMessageRepository
    {
        private readonly SocialOCDBContext _dbContext;
        public SQLMessageRepository(SocialOCDBContext dbContext) { 
            _dbContext = dbContext;
        }

        public async Task<List<Message>> getMessagesConversation(Guid conversationId, int page, int pageSize)
        {
            int skip = (page - 1) * pageSize;

            var messagesDomain = await _dbContext.Messages
                .Where(i => i.ConversationId == conversationId)
                .OrderByDescending(i => i.CreatedAt)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return messagesDomain;
        }

        public async Task<Message> SendMessage(Message message)
        {
            var messageDomain = await _dbContext.Messages.AddAsync(message);
            await _dbContext.SaveChangesAsync();
            return message;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using social_oc_api.Data;
using social_oc_api.Models.Domain.Chat;
using social_oc_api.Models.DTO.Chats;

namespace social_oc_api.Repositories.Chat
{
    public class SQLConversationRepository : IConversationRepository
    {
        private readonly SocialOCDBContext _dbContext;
        private readonly ITokenRepository _tokenRepository;

        public SQLConversationRepository(SocialOCDBContext dbContext, ITokenRepository tokenRepository)
        {
            _dbContext = dbContext;
            _tokenRepository = tokenRepository;
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

        public async Task<List<ConversationListDto>> GetConversations(string userOneId, int page, int pageSize)
        {
            int skip = (page - 1) * pageSize;

            var conversations = await _dbContext.Conversations.Include(c => c.ParticipantSecond)
                .Where(i => i.ParticipantOneId == userOneId)
                .Select(i => new ConversationListDto
                {
                    Id = i.Id,
                    ParticipantSecond = new ConversationParticipantSecond
                    {
                        Username = i.ParticipantSecond.UserName,
                        Name = i.ParticipantSecond.Name,
                        ImageProfile = i.ParticipantSecond.ImageProfile.FilePath
                    },
                    TokenConversation = _tokenRepository.GenerateConversationToken(i.Id, new[] {i.ParticipantOneId, i.ParticipantSecondId}),

                })
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();


            var messagesDomain = new List<Message> { };

            for (int i = 0; i < conversations.Count; i++)
            {
                var messageDomain = await _dbContext.Messages
                    .Where(m => m.ConversationId == conversations[i].Id)
                    .Skip(skip)
                    .Take(pageSize)
                    .OrderBy(i => i.CreatedAt)
                    .ToListAsync();

                if (messagesDomain.Count > 0)
                {
                    conversations[i].MessageList = new MessageList
                    {
                        LastMessage = messageDomain[0].MessageText,
                        ParticipantId = messageDomain[0].SenderId,
                        LastMessageTime = messageDomain[0].CreatedAt,
                        IsRead = messageDomain[0].IsRead,
                    };

                }
            }
            return conversations;
        }
    }
}

using Microsoft.AspNetCore.SignalR;
using social_oc_api.Models.Domain.Chat;
using social_oc_api.Repositories.Chat;
using social_oc_api.Repositories;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using social_oc_api.Models.DTO.Chats;

namespace social_oc_api.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {

        private readonly ILogger<ChatHub> _logger;
        private readonly ITokenRepository _tokenRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;

        public ChatHub(ILogger<ChatHub> logger, ITokenRepository tokenRepository, IMessageRepository messageRepository, IMapper mapper)
        {
            _logger = logger;
            _tokenRepository = tokenRepository;
            _messageRepository = messageRepository;
            _mapper = mapper;
        }
        public async Task SendMessage(string conversationId, string message, string conversationToken)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Unauthorized user tried to send a message");
                return;
            }

            if (!_tokenRepository.AreYouFromConversation(userId, conversationToken))
            {
                _logger.LogWarning("User {UserId} is not authorized for conversation {ConversationId}", userId, conversationId);
                return;
            }

            var newMessage = new Message
            {
                ConversationId = Guid.Parse(conversationId),
                SenderId = userId,
                MessageText = message,
            };
            await _messageRepository.SendMessage(newMessage);

            await Clients.Group(conversationId).SendAsync("ReceiveMessage", _mapper.Map<MessageDto>(newMessage));
        }
        public async Task JoinConversation(string conversationId, string conversationToken)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Unauthorized user tried to join a conversation");
                throw new HubException("Unauthorized");
            }

            if (!_tokenRepository.AreYouFromConversation(userId, conversationToken))
            {
                _logger.LogWarning("User {UserId} is not authorized for conversation {ConversationId}", userId, conversationId);
                throw new HubException("You are not authorized for this conversation.");
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
            _logger.LogInformation("Connection {ConnectionId} joined group {ConversationId}", Context.ConnectionId, conversationId);
        }
        public async Task LeaveConversation(string conversationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);
            _logger.LogInformation("Connection {ConnectionId} left group {ConversationId}", Context.ConnectionId, conversationId);
        }
    }
}

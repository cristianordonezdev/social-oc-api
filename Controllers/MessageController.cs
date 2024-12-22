using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using social_oc_api.Models.Domain.Chat;
using social_oc_api.Models.DTO.Chats;
using social_oc_api.Repositories;
using social_oc_api.Repositories.Chat;
using social_oc_api.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace social_oc_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageRepository _messageRepository;
        private readonly ITokenRepository _tokenRepository;
        private readonly IUtils _utils;
        private readonly IMapper _mapper;
        public MessageController(IMessageRepository messageRepository, ITokenRepository tokenRepository, IUtils utils, IMapper mapper)
        {
            _messageRepository = messageRepository;
            _tokenRepository = tokenRepository;
            _utils = utils;
            _mapper = mapper;
        }


        [HttpPost]
        [Authorize]
        [Route("{ConversationId}", Name = "Post a message")]
        public async Task<IActionResult> PostMessage([FromRoute] Guid ConversationId, [FromBody] SendMessageDto sendMessageDto, [FromHeader(Name = "Conversation-Token")] string conversationToken)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) { return Unauthorized(); }

            if (!_tokenRepository.AreYouFromConversation(userId, conversationToken))
            {
                ModelState.AddModelError("Message", "You are not in this conversation");
                return BadRequest(_utils.BuildErrorResponse(ModelState));
            }


            Message newMessage = new Message
            {
                ConversationId = ConversationId,
                SenderId = userId,
                MessageText = sendMessageDto.TextMessage,
            };

            var messageDomain = await _messageRepository.SendMessage(newMessage);

            return Ok(_mapper.Map<MessageDto>(messageDomain));
        }

        [HttpGet]
        [Authorize]
        [Route("{ConversationId}", Name = "Get messages from a conversation")]
        public async Task<IActionResult> GetMessages([FromRoute] Guid ConversationId, [FromHeader(Name = "Conversation-Token")] string conversationToken, [FromQuery] int page = 1, [FromQuery] int pageSize = 1)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) { return Unauthorized(); }

            if (!_tokenRepository.AreYouFromConversation(userId, conversationToken))
            {
                ModelState.AddModelError("Message", "You are not in this conversation");
                return BadRequest(_utils.BuildErrorResponse(ModelState));
            }
            var messagesDomain = await _messageRepository.getMessagesConversation(ConversationId, page, pageSize);

            return Ok(_mapper.Map<List<MessageDto>>(messagesDomain));
        }

    }
}

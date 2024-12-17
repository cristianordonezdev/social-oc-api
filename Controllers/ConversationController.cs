using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using social_oc_api.Models.Domain.Chat;
using social_oc_api.Models.DTO.Chats;
using social_oc_api.Repositories.Chat;
using social_oc_api.Utils;
using System.Security.Claims;

namespace social_oc_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConversationController : ControllerBase
    {
        private readonly IConversationRepository _conversationRepository;
        private readonly IMapper _mapper;
        private readonly IUtils _utils;
        public ConversationController(IConversationRepository conversationRepository, IMapper mapper, IUtils utils) { 
            _conversationRepository = conversationRepository;
            _mapper = mapper;
            _utils = utils;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetConversations()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) { return Unauthorized(); }

            var conversationsDomains = await _conversationRepository.GetConversations(userId);
            return Ok(conversationsDomains);
        }

        [HttpPost]
        [Authorize]
        [Route("{participantSecondId}", Name = "Post a conversation")]
        public async Task<IActionResult> PostConversation([FromRoute] string participantSecondId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) { return Unauthorized(); }

            var existConversation = await _conversationRepository.GetConversation(userId, participantSecondId);
            if (!existConversation)
            {
                Conversation conversation = new Conversation
                {
                    ParticipantOneId = userId,
                    ParticipantSecondId = participantSecondId
                };
                var conversationDomain = await _conversationRepository.CreateConversation(conversation);
                return Ok(_mapper.Map<ConversationDto>(conversationDomain));
            } else
            {
                ModelState.AddModelError("Conversation", "Conversation already created");
                return BadRequest(_utils.BuildErrorResponse(ModelState));
            }
        }

    }
}

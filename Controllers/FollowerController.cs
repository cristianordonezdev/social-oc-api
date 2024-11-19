using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using social_oc_api.Models.Domain;
using social_oc_api.Models.DTO.Auth;
using social_oc_api.Models.DTO.Followers;
using social_oc_api.Models.Responses;
using social_oc_api.Repositories;
using social_oc_api.Repositories.User;
using social_oc_api.Utils;
using System.Security.Claims;

namespace social_oc_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowerController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IFollowerRepository _followerRepository;
        private readonly IUtils _utils;
        private readonly IMapper _mapper;
        public FollowerController(IUserRepository userRepository, IFollowerRepository followerRepository, IUtils utils, IMapper mapper)
        {
            _followerRepository = followerRepository;
            _utils = utils;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("{followingId}", Name = "handleActionFollow")]
        [Authorize]

        public async Task<IActionResult> HandleActionFollow([FromRoute] string followingId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var userToFollow = await _userRepository.GetUserByUUID(followingId);
            if (userToFollow == null)
                return NotFound();

            var follower = new Follower
            {
                FollowerId = userId,
                FollowingId = followingId,
            };

            if (userToFollow.IsPublic)
            {
                var followerDomain = await _followerRepository.ToggleFollowAction(follower);
                return Ok(followerDomain == null ? followerDomain : _mapper.Map<FollowerDto>(followerDomain));
            }

            var isRequestFollow = await _followerRepository.HandleGetRequest(follower);
            var response = new OkResponse
            {
                Status = 200,
                Message = isRequestFollow ? "Request Follow" : "Unrequest Follow",
                Code = isRequestFollow ? "request_follow" : "unrequest_follow"
            };

            return Ok(response);
        }
        [HttpGet]
        [Route("requests/", Name = "Request follow user")]
        [Authorize]

        public async Task<IActionResult> getRequestsFollows([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var requests = await _followerRepository.GetListOfRequests(userId, page, pageSize);

            return Ok(requests);
        }

        [HttpDelete]
        [Route("requests/{requestId}", Name = "Delete Request")]
        [Authorize]

        public async Task<IActionResult> deleteRequest([FromRoute] Guid requestId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var requests = await _followerRepository.deleteRequest(requestId, userId);
            if (requests == null) return NotFound();

            return NoContent();
        }

        [HttpGet]
        [Route("requests/accept/{requestId}", Name = "Accept Request")]
        [Authorize]
        public async Task<IActionResult> acceptRequest([FromRoute] Guid requestId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var requests = await _followerRepository.acceptRequest(requestId, userId);
            if (requests == null) return NotFound();

            return Ok(_mapper.Map<FollowerDto>(requests));
        }
    }
}

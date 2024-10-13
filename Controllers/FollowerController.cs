using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using social_oc_api.Models.Domain;
using social_oc_api.Repositories;
using social_oc_api.Utils;
using System.Security.Claims;

namespace social_oc_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowerController : ControllerBase
    {
        private readonly IFollowerRepository _followerRepository;
        private readonly IUtils _utils;
        public FollowerController(IFollowerRepository followerRepository, IUtils utils)
        {
            _followerRepository = followerRepository;
            _utils = utils;
        }

        [HttpGet]
        [Route("{followingId}", Name = "handleActionFollow")]
        [Authorize]

        public async Task<IActionResult> HandleActionFollow([FromRoute] string followingId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId)) { return Unauthorized(); }

            var follower = new Follower
                {
                    FollowerId = userId,
                    FollowingId = followingId,
                };

            var followerDomain = await _followerRepository.ToggleFollowAction(follower);

            return Ok(followerDomain);
        }
    }
}

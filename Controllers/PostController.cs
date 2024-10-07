using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using social_oc_api.Models.DTO.Posts;
using social_oc_api.Models.Domain;
using System.Security.Claims;
using social_oc_api.Repositories;
using social_oc_api.Utils;
namespace social_oc_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IPostRepository _postRepository;
        private readonly IUtils _utils;
        public PostController(IMapper mapper, IPostRepository postRepository, IUtils utils)
        {
            _mapper = mapper;
            _postRepository = postRepository;
            _utils = utils;
        }
        [HttpPost]
        [Authorize]

        public async Task<IActionResult> Post([FromForm] PostCreateDto postCreateDto)
        {
            _utils.ValidateFileUpload(postCreateDto.File, ModelState);

            if (ModelState.IsValid)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId != null)
                {
                    var postDomain = _mapper.Map<Post>(postCreateDto);
                    postDomain.UserId = new Guid(userId);

                    var postDomainSaved = await _postRepository.CreatePost(postDomain, postCreateDto.File);
                    return Ok(_mapper.Map<PostDto>(postDomainSaved));
                }
                return NotFound();
            }
            var errorResponse = _utils.BuildErrorResponse(ModelState);
            return BadRequest(errorResponse);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> getPostsHome()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                var postsDomain = await _postRepository.GetPostsHome(new Guid(userId));
                return Ok(_mapper.Map<List<PostDto>>(postsDomain));
            }

            ModelState.AddModelError("Home post", "Something wrong happened when it was trying to get home posts");
            var errorResponse = _utils.BuildErrorResponse(ModelState);

            return BadRequest(errorResponse);
        }

        [HttpGet]
        [Authorize]
        [Route("Of/{userId?}", Name = "GetPostsOf")]

        public async Task<IActionResult> getPostsOf([FromRoute] string? userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }
            var postsDomain = await _postRepository.GetPostsOf(new Guid(userId));
            return Ok(_mapper.Map<List<PostDto>>(postsDomain));
        }
    }
}

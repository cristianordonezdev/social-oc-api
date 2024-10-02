using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using social_oc_api.Models.DTO.Posts;
using social_oc_api.Models.Domain;
using System.Security.Claims;
using social_oc_api.Repositories;
namespace social_oc_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IPostRepository _postRepository;
        public PostController(IMapper mapper, IPostRepository postRepository)
        {
            _mapper = mapper;
            _postRepository = postRepository;
        }
        [HttpPost]
        [Authorize]

        public async Task<IActionResult> Post([FromForm] PostCreateDto postCreateDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                var postDomain = _mapper.Map<Post>(postCreateDto);
                postDomain.UserId = new Guid(userId);

                postDomain.Files = postCreateDto.Files.Select(file => new Image
                {
                    File = file,
                    FilePath = "hello wolrd"
                }).ToList();

                var postDomainSaved = await _postRepository.CreatePost(postDomain);
                return Ok(_mapper.Map<PostDto>(postDomainSaved));
            }


            return Ok();
        }

        [HttpGet]
        [Authorize]

        public async Task<IActionResult> getPostsHome()
        {
            var postsDomain = await _postRepository.GetPosts();
            return Ok(_mapper.Map<List<PostDto>>(postsDomain));
        }
    }
}

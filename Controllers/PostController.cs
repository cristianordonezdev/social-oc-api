using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using social_oc_api.Models.DTO.Posts;
using social_oc_api.Models.Domain;
using System.Security.Claims;
using social_oc_api.Repositories;
using social_oc_api.Utils;
using social_oc_api.Repositories.User;
using social_oc_api.Models.DTO.Auth;
using social_oc_api.Models.Responses;
namespace social_oc_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;
        private readonly IFollowerRepository _followerRepository;
        private readonly IUtils _utils;
        public PostController(IMapper mapper, IPostRepository postRepository, IUtils utils, IUserRepository userRepository, IFollowerRepository followerRepository)
        {
            _mapper = mapper;
            _postRepository = postRepository;
            _utils = utils;
            _userRepository = userRepository;
            _followerRepository = followerRepository;
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> getPostsHome([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId)) { return Unauthorized(); }

            var postsDomain = await _postRepository.GetPostsHome(userId, page, pageSize);
            return Ok(postsDomain);
           
        }

        [HttpGet]
        [Authorize]
        [Route("Of/{username}", Name = "GetPostsOf")]
        public async Task<IActionResult> getPostsOf([FromRoute] string username, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) { return Unauthorized(); }
            if (string.IsNullOrEmpty(username)) { return NotFound(); }

            var profileUser = await _userRepository.GetProfile(username);
            if (profileUser == null) { return NotFound(); }

            bool isVisible = profileUser.User.IsPublic ? profileUser.User.IsPublic : await _followerRepository.GetVisibility(profileUser.User.Id, userId);

            if (isVisible)
            {
                var postsDomain = await _postRepository.GetPostsOf(profileUser.User.Id, page, pageSize);
                return Ok(postsDomain);
            }

            return Ok(new OkResponse
            {
                Status = 200,
                Message = "Privated Profile",
                Code = "privated_profile"
            });
        }

        [HttpGet]
        [Authorize]
        [Route("comments/{postId}", Name = "getComment")]
        public async Task<IActionResult> getComments([FromRoute] string postId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrEmpty(postId)) { return NotFound(); }

            var commentsDto = await _postRepository.GetCommentsPosts(new Guid(postId), page, pageSize);
            return Ok(commentsDto);
        }



        [HttpGet]
        [Authorize]
        [Route("{postId}", Name = "getPostDetail")]
        public async Task<IActionResult> postDetail([FromRoute] string postId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) { return Unauthorized(); }

            var post = await _postRepository.GetPostDetail(new Guid(postId), userId);
            if (post == null) { return NotFound(); }

            if (post.IsVisible)
            {
                return Ok(post); ;
            }

            return Ok(new OkResponse
            {
                Status = 200,
                Message = "Privated Profile",
                Code = "privated_profile"
            });
        }

        [HttpGet]
        [Authorize]
        [Route("like/{postId}", Name = "likePost")]
        public async Task<IActionResult> likePost([FromRoute] string postId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) { return Unauthorized(); }

            var like = await _postRepository.LikePost(userId, new Guid(postId));
            if (like == null) { return NotFound(); }

            return Ok(_mapper.Map<LikePostDto>(like));
        }

        [HttpGet]
        [Authorize]
        [Route("likes/{postId}", Name = "likes Posts")]
        public async Task<IActionResult> getLikes([FromRoute] string postId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) { return Unauthorized(); }

            var likes = await _postRepository.LikesUsers(new Guid(postId), userId, page, pageSize);
            if (likes == null) { return NotFound(); }

            return Ok(likes);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromForm] PostCreateDto postCreateDto)
        {
            _utils.ValidateFileUpload(postCreateDto.File, ModelState, null);

            if (ModelState.IsValid)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId)) { return Unauthorized(); }

                var postDomain = _mapper.Map<Post>(postCreateDto);
                postDomain.UserId = userId;

                var postDomainSaved = await _postRepository.CreatePost(postDomain, postCreateDto.File);
                return Ok(_mapper.Map<PostDto>(postDomainSaved));
            }
            var errorResponse = _utils.BuildErrorResponse(ModelState);
            return BadRequest(errorResponse);
        }

        [HttpPost]
        [Authorize]
        [Route("comment/{postId?}")]
        public async Task<IActionResult> PostComment([FromBody] CommentDto commentDto, [FromRoute] Guid postId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) { return Unauthorized(); }

            var commentDtoFull = await _postRepository.CommentPost(userId, postId, commentDto.CommentText);
            
            return Ok(commentDtoFull);
        }


        [HttpPut]
        [Authorize]
        [Route("{postId?}")]

        public async Task<IActionResult> UpdatePost([FromForm] UpdatePostDto updatePostDto, [FromRoute] Guid postId)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) { return Unauthorized(); }

            var postDomainSaved = await _postRepository.UpdatePost(updatePostDto.Caption, postId, userId);
            if (postDomainSaved != null)
            {
                return Ok(postDomainSaved);
            } else { return NotFound();  }
 
        }

        [HttpDelete]
        [Authorize]
        [Route("{postId?}")]

        public async Task<IActionResult> deletePost([FromRoute] Guid postId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) { return Unauthorized(); }

            var postDomain = await _postRepository.deletePost(postId, new Guid(userId));
            if (postDomain == null) { return NotFound(); }
            return Ok(_mapper.Map<PostDto>(postDomain));
        }

        [HttpDelete]
        [Authorize]
        [Route("comment/{commentId?}")]
        public async Task<IActionResult> deleteComment([FromRoute] Guid commentId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) { return Unauthorized(); }

            var isDeleted = await _postRepository.deleteComment(commentId, userId);
            if (isDeleted == null) { return NotFound(); }
            return NoContent();
        }

        [HttpDelete]
        [Authorize]
        [Route("image/{imageId?}")]
        public async Task<IActionResult> deleteImagePost([FromRoute] Guid imageId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) { return Unauthorized(); }

            var isDeleted = await _postRepository.deleteImagePost(imageId, userId);
            if (isDeleted == null) { return NotFound(); }
            else if (isDeleted == false) {
                ModelState.AddModelError("delete_image", "Cannot remove all images");
                return BadRequest(_utils.BuildErrorResponse(ModelState));
            }
            return NoContent();
        }
    }
}

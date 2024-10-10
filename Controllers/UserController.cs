using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using social_oc_api.Models.Domain;
using social_oc_api.Models.Domain.Images;
using social_oc_api.Models.DTO.Auth;
using social_oc_api.Models.DTO.Posts;
using social_oc_api.Repositories.User;
using social_oc_api.Utils;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace social_oc_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IUtils _utils;
        public UserController(IUserRepository userRepository, UserManager<ApplicationUser> userManage, IMapper mapper, IUtils utils)
        {
            _userRepository = userRepository;
            _userManager = userManage;
            _mapper = mapper;
            _utils = utils;
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> deleteUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) { return Unauthorized(); }

            var deletedUser = await _userRepository.DeleteUser(new Guid(userId));
            if (deletedUser) { return Ok(); }
            else { return BadRequest(); }
        }


        [HttpPost]
        [Authorize]
        [Route("profilePicture")]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile file)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) { return Unauthorized(); }

            _utils.ValidateFileUpload([file], ModelState, [".jpg", ".jpeg", ".png"]);

            if (ModelState.IsValid)
            {
                var UserImage = new UserImage
                {
                    File = file,
                    UserId = userId
                };

                var userDomain = await _userRepository.UploadImageProfile(UserImage);

                return Ok(_mapper.Map<UserDto>(userDomain));
            }

            var errorResponse = _utils.BuildErrorResponse(ModelState);
            return BadRequest(errorResponse);
        }
    }
}

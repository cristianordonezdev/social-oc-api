using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using social_oc_api.Models.Domain;
using social_oc_api.Models.Domain.Images;
using social_oc_api.Repositories.User;
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

        public UserController(IUserRepository userRepository, UserManager<ApplicationUser> userManage)
        {
            _userRepository = userRepository;
            _userManager = userManage;   
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

            var UserImage = new UserImage
            {
                File = file,
                UserId = userId
            };

            var userDomain = await _userRepository.UploadImageProfile(UserImage);

            return Ok(userDomain);
        }
    }
}

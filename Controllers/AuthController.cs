using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using social_oc_api.Repositories;
using social_oc_api.Models.DTO.Auth;
using social_oc_api.Models.Domain;
using social_oc_api.Utils;

namespace social_oc_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ITokenRepository tokenRepository;
        private readonly IUtils _utils;

        public AuthController(UserManager<ApplicationUser> userManager, ITokenRepository tokenRepository, IUtils utils)
        {
            this.userManager = userManager;
            this.tokenRepository = tokenRepository;
            this._utils = utils;
        }

        // POST: /api/auth/register
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(_utils.BuildErrorResponse(ModelState));
            }

            var identityUser = new ApplicationUser
            {
                UserName = registerRequestDto.Username,
                Email = registerRequestDto.Email,
                Name = registerRequestDto.Name,
            };

            var identityResult = await userManager.CreateAsync(identityUser, registerRequestDto.Password);

            if (identityResult.Succeeded)
            {
                List<string> roles = new List<string> { "Reader" };
                identityResult = await userManager.AddToRolesAsync(identityUser, roles);
                if (identityResult.Succeeded)
                {
                    return Ok("User was registered, please login");
                }
            }
            var resultError = identityResult != null && identityResult.Errors.Any() ? identityResult.Errors.FirstOrDefault()?.Description ?? "An unknown error occurred." : "Something wrong happen";

            ModelState.AddModelError("Register", resultError);

            var errorResponse = _utils.BuildErrorResponse(ModelState);

            return BadRequest(errorResponse);
        }
    }
}

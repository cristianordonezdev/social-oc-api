using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using social_oc_api.Repositories;
using social_oc_api.Models.DTO.Auth;
using social_oc_api.Models.Domain;
using social_oc_api.Utils;
using social_oc_api.Models.Responses;

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

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(_utils.BuildErrorResponse(ModelState));
            }

            var user = await userManager.FindByEmailAsync(registerRequestDto.Email);
            if (user != null)
            {
                ModelState.AddModelError("Register", "Email '" + registerRequestDto.Email + "' is already taken.");
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
                    return Ok(new OkResponse
                    {
                        Status = 200,
                        Message = "The user has been created, please log in"
                    });
                }
            }
            var resultError = identityResult != null && identityResult.Errors.Any() ? identityResult.Errors.FirstOrDefault()?.Description ?? "An unknown error occurred." : "Something wrong happen";
            ModelState.AddModelError("Register", resultError);
            var errorResponse = _utils.BuildErrorResponse(ModelState);

            return BadRequest(errorResponse);
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var user = await userManager.FindByEmailAsync(loginRequestDto.Username);
            if (user != null)
            {
                var checkPassword = await userManager.CheckPasswordAsync(user, loginRequestDto.Password);
                if (checkPassword)
                {
                    // Generate token

                    var roles = await userManager.GetRolesAsync(user);
                    if (roles != null)
                    {
                        var jwtToken = tokenRepository.CreateJWTToken(user, roles.ToList());

                        var response = new LoginResponseDto
                        {
                            Token = jwtToken,
                            NameUser = user.UserName,
                        };

                        return Ok(response);
                    }

                }
            }

            ModelState.AddModelError("Login", "Invalid email or password");
            var errorResponse = _utils.BuildErrorResponse(ModelState);

            return BadRequest(errorResponse);
        }
    }
}

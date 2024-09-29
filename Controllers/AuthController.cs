using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using social_oc_api.Repositories;
using social_oc_api.Models.DTO.Auth;
using social_oc_api.Models.Domain;
using social_oc_api.Utils;
using social_oc_api.Models.Responses;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Authorization;

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
            var user = await userManager.FindByEmailAsync(loginRequestDto.Email);
            if (user != null)
            {
                var checkPassword = await userManager.CheckPasswordAsync(user, loginRequestDto.Password);
                if (checkPassword)
                {
                    var roles = await userManager.GetRolesAsync(user);
                    if (roles != null)
                    {
                        var jwtToken = tokenRepository.CreateJWTToken(user, roles.ToList(), 1440);
                        var refreshToken = tokenRepository.CreateJWTToken(user, roles.ToList(), 10080);

                        if (!string.IsNullOrEmpty(user.Id))
                        {
                           await tokenRepository.SaveRefreshToken(user.Id, refreshToken);
                        }

                        var response = new LoginResponseDto
                        {
                            Token = jwtToken,
                            RefreshToken = refreshToken,
                            NameUser = user.UserName
                        };

                        return Ok(response);
                    }
                }
            }

            ModelState.AddModelError("Login", "Invalid email or password");
            return BadRequest(_utils.BuildErrorResponse(ModelState));
        }

        [HttpPost]
        [Route("RefreshToken")]
        public async Task<IActionResult> RefreshToken(TokenRefreshRequest tokenRequest)
        {
            var principal = tokenRepository.GetPrincipalFromExpiredToken(tokenRequest.Token);
            if (principal == null)
            {
                ModelState.AddModelError("Refresh Token", "Invalid refresh token");
                return BadRequest(_utils.BuildErrorResponse(ModelState));
            }
            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var savedRefreshToken = await tokenRepository.GetRefreshToken(new Guid(userId), tokenRequest.Token);
            if (savedRefreshToken == null)
            {
                return NotFound();
            }

            if (savedRefreshToken.ExpiresAt < DateTime.Now)
            {
                ModelState.AddModelError("Refresh Token", "Expired refresh token");
                return BadRequest(_utils.BuildErrorResponse(ModelState));
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null) { return NotFound(); }

            var roles = await userManager.GetRolesAsync(user);
            if (roles != null)
            {
                var jwtToken = tokenRepository.CreateJWTToken(user, roles.ToList(), 1440);
                var refreshToken = tokenRepository.CreateJWTToken(user, roles.ToList(), 10080);

                if (!string.IsNullOrEmpty(user.Id))
                {
                    await tokenRepository.SaveRefreshToken(user.Id, refreshToken);
                }

                var response = new LoginResponseDto
                {
                    Token = jwtToken,
                    RefreshToken = refreshToken,
                    NameUser = user.UserName
                };

                return Ok(response);
            }

            ModelState.AddModelError("Refresh Token", "Something wrong happened");
            return BadRequest(_utils.BuildErrorResponse(ModelState));
        }


        [HttpGet]
        [Route("user")]
        [Authorize]
        public async Task<IActionResult> GetUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpGet]
        [Route("GoogleLogin")]
        public IActionResult GoogleLogin(string redirectUri)
        {
            var redirectUrl = Url.Action("GoogleResponse", "Auth", new { redirectUri }, Request.Scheme);
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet]
        [Route("GoogleResponse")]
        public async Task<IActionResult> GoogleResponse(string redirectUri)
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (result.Succeeded)
            {
                var claims = result.Principal.Claims.ToList();
                var user = new ApplicationUser
                {
                    UserName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
                    Email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
                    Name = claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value,
                };

                var userFromDb = await userManager.FindByEmailAsync(user.Email);
                if (userFromDb == null) { NotFound(); }

                // Verifica si el usuario ya existe, si no, crea uno nuevo.
                var existingUser = await userManager.FindByEmailAsync(user.Email);
                if (existingUser == null)
                {
                    await userManager.CreateAsync(user);
                    await userManager.AddToRoleAsync(user, "Reader");
                }

                var roles = await userManager.GetRolesAsync(user);
                var jwtToken = tokenRepository.CreateJWTToken(user, roles.ToList(), 1440);
                var refreshToken = tokenRepository.CreateJWTToken(user, roles.ToList(), 10080);

                await tokenRepository.SaveRefreshToken(userFromDb.Id, refreshToken);
                

                return Ok(new { Token = jwtToken, RefreshToken = refreshToken, NameUser = user.UserName });
            }


            ModelState.AddModelError("Login Google", "Login Failed");
            return BadRequest(_utils.BuildErrorResponse(ModelState));
        }

    }
}

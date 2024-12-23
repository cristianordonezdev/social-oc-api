﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using social_oc_api.Data;
using social_oc_api.Models.Domain.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace social_oc_api.Repositories
{
    public class SQLTokenRepository : ITokenRepository
    {
        private readonly IConfiguration configuration;
        private readonly SocialOCDBContext _db_context;
        public SQLTokenRepository(IConfiguration configuration, SocialOCDBContext dbContext)
        {
            this.configuration = configuration;
            this._db_context = dbContext;
        }

        public bool AreYouFromConversation(string userId, string conversationToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(conversationToken);

            var conversationId = jwtToken.Claims.First(claim => claim.Type == "conversationId").Value;
            var participants = jwtToken.Claims.First(claim => claim.Type == "participants").Value.Split(',');

            return participants.Contains(userId);
        }

        public string CreateJWTToken(IdentityUser user, List<string> roles, int minutes)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                configuration["Jwt:Issuer"],
                configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(minutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateConversationToken(Guid conversationId, string[] participants)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
                {
                    new Claim("conversationId", conversationId.ToString()),
                    new Claim("participants", string.Join(",", participants))
                };

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            try
            {

                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
                    ValidateLifetime = false
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

                if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new SecurityTokenException("Invalid token");
                }

                return principal;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<RefreshToken?> GetRefreshToken(string UserId, string Token)
        {
            var refreshToken = await _db_context.RefreshTokens.FirstOrDefaultAsync(t => t.UserId == UserId && Token == t.Token);
            return refreshToken;
        }

        public async Task SaveRefreshToken(string UserId, string Token)
        {

            var userToken = await _db_context.RefreshTokens.FirstOrDefaultAsync((i) => i.UserId.Equals(UserId));
            if (userToken == null)
            {
                var newRefreshToken = new RefreshToken
                {
                    UserId = UserId,
                    Token = Token,
                    ExpiresAt = DateTime.Now.AddMinutes(10080),
                    IsRevoked = false
                };

                _db_context.RefreshTokens.Add(newRefreshToken);
            } else
            {
                userToken.Token = Token;
                userToken.ExpiresAt = DateTime.Now.AddMinutes(10080);
            }

            await _db_context.SaveChangesAsync();
        }
    }
}

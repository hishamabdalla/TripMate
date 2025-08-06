using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Application.Services.Abstractions.Identity;
using Tripmate.Domain.AppSettings;
using Tripmate.Domain.Common.Response;
using Tripmate.Domain.Entities.Models;

namespace Tripmate.Application.Services.Identity.Token
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings JwtSettings;
        private readonly UserManager<ApplicationUser> _userManager;
        public TokenService(IOptions<JwtSettings> options, UserManager<ApplicationUser> userManager)
        {
            JwtSettings = options.Value;
            _userManager = userManager;
        }
        public async Task<TokenResponse> GenerateTokenAsync(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim("userName", user.UserName),
                new Claim("userId", user.Id.ToString()),
                new Claim("email", user.Email),

                // Add other claims as needed
            };

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSettings.Secret));

            var token = new JwtSecurityToken(
                issuer: JwtSettings.Issuer,
                audience: JwtSettings.Audience,
                expires: DateTime.UtcNow.AddHours(JwtSettings.ExpirationHours),
                claims: claims,
                signingCredentials:new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)


            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return new TokenResponse
            {
                AccessToken = tokenString,
                ExpiresIn=token.ValidTo,
                TokenType = "Bearer",
                RefreshToken= Guid.NewGuid().ToString() 

            };




        }

      
    }
}

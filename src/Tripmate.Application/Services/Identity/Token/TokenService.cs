using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Tripmate.Application.Services.Abstractions.Identity;
using Tripmate.Domain.AppSettings;
using Tripmate.Domain.Common.Response;
using Tripmate.Domain.Entities.Models;
using System.Security.Cryptography;

namespace Tripmate.Application.Services.Identity.Token
{
    public class TokenService(IOptions<JwtSettings> options, UserManager<ApplicationUser> userManager)
        : ITokenService
    {
        private readonly JwtSettings JwtSettings = options.Value;

        public async Task<TokenResponse> GenerateTokenAsync(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                    
                // Add other claims as needed
            };

            var userRoles = await userManager.GetRolesAsync(user);
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
            };

        }

        public RefreshToken GenerateRefreshToken()
        {
            // Generate a random string for the refresh token
            var randomNumber = new byte[32];
            using var generator = new RNGCryptoServiceProvider();
            generator.GetBytes(randomNumber);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                Expiration = DateTime.UtcNow.AddDays(7) // Set expiration for 7 days
            };


        }


    }
}

using IdentityService.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace IdentityService.Services.Implementation
{
    public class IdentityService : IIdentityService
    {
        private readonly IConfiguration _configuration;

        public IdentityService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<LoginResponse> Authenticate(string username, string password)
        {
            //Todo - Validate credentials against DB

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var expires = DateTime.UtcNow.AddHours(1);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return await Task.FromResult(new LoginResponse
            {
                Username = username,
                AccessToken = tokenString,
                ExpiresIn = (int)(expires - DateTime.UtcNow).TotalSeconds
            });
        }
    }
} 
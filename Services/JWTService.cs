using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CareSync.Services
{
    public class JWTService
    {
        private readonly string _secretKey;

        public JWTService(IConfiguration configuration)
        {
            _secretKey = configuration["Jwt:SecretKey"];
        }

        public string GenerateToken(string userId, string firstName, string lastName, string userType)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, $"{firstName} {lastName}"),
                new Claim(ClaimTypes.Role, userType)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "CareSync",
                audience: "CareSyncUsers",
                claims: claims,
                expires: null,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

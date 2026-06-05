using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CarCareTracker.Models;
using Microsoft.IdentityModel.Tokens;

namespace CarCareTracker.Services
{
    /// <summary>
    /// Handles creation and validation of JWT tokens.
    /// (Web Authentication requirement - Software Development course)
    /// </summary>
    public class JwtService
    {
        private static string Key => ConfigurationManager.AppSettings["Jwt:Key"];
        private static string Issuer => ConfigurationManager.AppSettings["Jwt:Issuer"];
        private static string Audience => ConfigurationManager.AppSettings["Jwt:Audience"];
        private static int ExpireMinutes => int.Parse(ConfigurationManager.AppSettings["Jwt:ExpireMinutes"]);

        public string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.RoleName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(ExpireMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token)) return null;

            var handler = new JwtSecurityTokenHandler();
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));

            try
            {
                var parameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = Issuer,
                    ValidateAudience = true,
                    ValidAudience = Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = securityKey,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                SecurityToken validated;
                return handler.ValidateToken(token, parameters, out validated);
            }
            catch
            {
                return null;
            }
        }
    }
}

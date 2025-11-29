using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TFinanceWeb.Api.Models;
using Microsoft.IdentityModel.Tokens;

namespace TFinanceWeb.Api.Services;

public class AuthUserService
{
    public string GenerateJwtToken(string username, string email, string id)
    {
        var secretKey = Environment.GetEnvironmentVariable("JWT_KEY");
        var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
        var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");
        var expiresInHours = int.TryParse(
            Environment.GetEnvironmentVariable("JWT_EXPIRES_IN_HOURS"), out var hours);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(s: secretKey!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Role, "User"),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.NameIdentifier, id),
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(hours),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = credentials
        };
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
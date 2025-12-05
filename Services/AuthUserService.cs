using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TFinanceWeb.Api.Models;
using Microsoft.IdentityModel.Tokens;

namespace TFinanceWeb.Api.Services;

public class AuthUserService
{
    public string GenerateJwtToken(string username, string email, string id, string role = "User")
    {
        // Получение настроек с проверками
        var secretKey = Environment.GetEnvironmentVariable("JWT_KEY");
        var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
        var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");
        var expiresInHoursStr = Environment.GetEnvironmentVariable("JWT_EXPIRES_IN_HOURS");

        if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
        {
            throw new InvalidOperationException("Отсутствуют необходимые настройки JWT в переменных окружения.");
        }

        // Парсинг expires с fallback (по умолчанию 1 час)
        if (!int.TryParse(expiresInHoursStr, out var hours) || hours <= 0)
        {
            hours = 1; // Fallback: 1 час
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Role, role), // Теперь параметр
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Email, email),
            new Claim("sub", id),
            // Можно добавить больше: new Claim("custom", "value")
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(hours),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = credentials
        };

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Ошибка при генерации токена.");
        }
    }
}
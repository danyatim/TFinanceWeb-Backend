using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TFinanceWeb.Api.Data;
using TFinanceWeb.Api.Models;
using TFinanceWeb.Api.Services;
using TFinanceWeb.Api.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace TFinanceWeb.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    FinanceWebDbContext context,
    AuthUserService authUserService,
    IWebHostEnvironment environment,
    ILogger<AuthController> logger
    ) : ControllerBase
{
    private readonly AuthUserService _authUserService = authUserService;
    private readonly FinanceWebDbContext _context = context;
    private readonly IWebHostEnvironment _env =  environment;
    private readonly ILogger<AuthController> _logger =  logger;
    
    public record RegisterRequest(
        string Login,
        string Username,
        string Email,
        string Password,
        string ConfirmPassword
        );
    
    public record LoginRequest(
        string Login,
        string Password
        );
    
    public record LoginResponse(
        string Username,
        string Email
        );
    
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var login = request.Login.Trim();
        var username = request.Username.Trim();
        var email = request.Email.Trim();
        var password = request.Password.Trim();
        var confirmPassword = request.ConfirmPassword.Trim();
        
        
        // Валидация входных данных
        var validation = ValidateRegister.ValidateRegisterRequest(
            login: login,
            username: username,
            email: email,
            password: password,
            confirmPassword: confirmPassword
        );
        
        if (!validation.isValid) {
            return BadRequest(validation.errorMessage);
        }
        
        // Проверка существования пользователя
        if (await _context.Users.AnyAsync(u => u.Email == email || u.Login == login)) {
            return Conflict("Пользователь с таким email или логином уже существует");
        }

        try {
            //Init User
            var user = new User {
                UserId = Guid.NewGuid(),
                Username = username,
                Login = login,
                Email = email,
                PasswordHash = string.Empty
            };

            //Init HasherPassword and add hash password 
            var hasher = new PasswordHasher<User>();
            user.PasswordHash = hasher.HashPassword(user, password);

            //Save user for database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            //Return result
            return Ok(new { message = "Регистрация успешна", username = user.Username });
        }
        
        catch (Exception ex) {
            //Log exception
            Console.WriteLine(ex);
            return StatusCode(500, "Ошибка при создании пользователя, попробуйте позже.");
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        // Валидация входных данных
        var validation = ValidateLogin.Login(
            login: request.Login.Trim(),
            password: request.Password.Trim()
        );
        if (!validation.isValid) {
            return BadRequest(validation.errorMessage);
        }
        
        // Проверка существования пользователя
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Login == request.Login);
        if (user == null) {
            return Unauthorized(new {message = "Неверный логин или пароль."});
        }
        
        // Проверка пароля
        var hasher = new PasswordHasher<User>();
        var result = hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (result == PasswordVerificationResult.Failed) {
            return Unauthorized(new {message = "Неверный логин или пароль."});
        }
        var token = _authUserService.GenerateJwtToken(user.Username, user.Email, user.Id.ToString());
        Response.Cookies.Append("uid", token, new CookieOptions
        {
            HttpOnly = true,
            Secure =  true,
            SameSite = _env.IsDevelopment() ? SameSiteMode.None : SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddHours(1)
        });
        
        // Успешный вход
        return Ok(new { message = "Вход выполнен успешно", user = new LoginResponse(user.Username, user.Email)});
    }

    [HttpGet($"validate")]
    public IActionResult ValidateToken()
    {
        var uid = Request.Cookies["uid"];
        if (string.IsNullOrWhiteSpace(uid))
        {
            return BadRequest(new { message = "Токен не предоставлен" });
        }
        // Получение настроек из env (с проверкой на null)
        var secretKey = Environment.GetEnvironmentVariable("JWT_KEY");
        var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
        var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");

        if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
        {
            return StatusCode(500, new { message = "Ошибка конфигурации сервера" }); // Не раскрываем детали
        }
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            tokenHandler.ValidateToken(uid, tokenValidationParameters, out var validatedToken);
            // Если нужно, извлечь claims (например, user ID)
            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            
            var user = _context.Users.Find(userId);
            if (user == null)
            {
                return NotFound(new { message = "Пользователь не найден" });
            }

            return Ok(new { message = "Токен валиден", user = new LoginResponse(user.Username, user.Email) });
        }
        catch (Exception ex)
        {
            return Unauthorized(new { message = "Токен недействителен" });
        }
        
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("uid");
        //Успешный выход
        return Ok(new { message = "Выход выполнен успешно"});
    }
}
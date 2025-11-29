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
    IWebHostEnvironment environment
    ) : ControllerBase
{
    private readonly AuthUserService _authUserService = authUserService;
    private readonly FinanceWebDbContext _context = context;
    private readonly IWebHostEnvironment _env =  environment;
    
    public record RegisterRequest(
        string Username,
        string Login,
        string Email,
        string Password,
        string ConfirmPassword
        );
    
    public record LoginRequest(
        string Login,
        string Password
        );
    
    [HttpPost("/register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        // Валидация входных данных
        var validation = ValidateRegister.ValidateRegisterRequest(
            login: request.Login.Trim(),
            username: request.Username.Trim(),
            email: request.Email.Trim(),
            password: request.Password.Trim(),
            confirmPassword: request.ConfirmPassword.Trim()
        );
        
        if (!validation.isValid) {
            return BadRequest(validation.errorMessage);
        }
        
        // Проверка существования пользователя
        if (await _context.Users.AnyAsync(u => u.Email == request.Email || u.Login == request.Login)) {
            return Conflict("Пользователь с таким email или логином уже существует");
        }

        try {
            //Init User
            var user = new User {
                UserId = Guid.NewGuid(),
                Username = request.Username,
                Login = request.Login,
                Email = request.Email,
                PasswordHash = string.Empty
            };

            //Init HasherPassword and add hash password 
            var hasher = new PasswordHasher<User>();
            user.PasswordHash = hasher.HashPassword(user, request.Password);

            //Save user for database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            //Return result
            return Ok(new { message = "Регистрация успешна", userId = user.UserId });
        }
        
        catch (Exception ex) {
            //Log exception
            Console.WriteLine(ex);
            return StatusCode(500, "Ошибка при создании пользователя, попробуйте позже.");
        }
    }

    [HttpPost("/login")]
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
        return Ok(new { message = "Вход выполнен успешно", username = user.Username });
    }

    [HttpPost("/logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("uid");
        //Успешный выход
        return Ok(new { message = "Выход выполнен успешно"});
    }
}
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TFinanceWeb.Api.Data;
using TFinanceWeb.Api.Models;
using TFinanceWeb.Api.Utils;

namespace TFinanceWeb.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(FinanceWebDbContext context) : Controller
{
    public record RegisterRequest(
        string Username,
        string Login,
        string Email,
        string Password,
        string ConfirmPassword
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
        
        if (!validation.isValid)
        {
            return BadRequest(validation.errorMessage);
        }
        
        // Проверка существования пользователя
        if (await context.Users.AnyAsync(u => u.Email == request.Email || u.Login == request.Login))
        {
            return Conflict("Пользователь с таким email или логином уже существует");
        }

        try
        {
            //Init User
            var user = new User
            {
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
            context.Users.Add(user);
            await context.SaveChangesAsync();
            
            //Return result
            return Ok(new { message = "Регистрация успешна", userId = user.UserId });
        }
        
        catch (Exception ex)
        {
            //Log exception
            Console.WriteLine(ex);
            return StatusCode(500, "Ошибка при создании пользователя, попробуйте позже.");
        }
    }
}
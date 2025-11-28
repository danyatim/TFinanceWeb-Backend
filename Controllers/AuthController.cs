using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TFinanceWeb.Api.Data;
using TFinanceWeb.Api.Models;

namespace TFinanceWeb.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(FinanceWebDbContext context) : Controller
{
    public record RegisterRequest(string Email, string Login, string Password);
    
    [HttpGet("/register")]
    public async Task<IActionResult> Login(RegisterRequest request)
    {
        //Init User
        var user = new User
        {
            UserId = Guid.NewGuid(),
            Username =  request.Login,
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
        return Ok("Login successful");
    }
}
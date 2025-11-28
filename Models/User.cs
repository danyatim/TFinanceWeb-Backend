using System.ComponentModel.DataAnnotations.Schema;

namespace TFinanceWeb.Api.Models;

[Table("Users")]
public class User
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; } = null!;
    public string Login { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public bool IsPremium { get; set; }
    public int PremiumPlan { get; set; }
    public DateTime PremiumCreateAt { get; set; }
    public DateTime PremiumExpiresAt { get; set; }
}
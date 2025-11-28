using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TFinanceWeb.Api.Utils;

namespace TFinanceWeb.Api.Models;

[Table("Users")]
public class User
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    [MaxLength(Constants.MaxUsernameLength)]
    
    public string Username { get; set; } = null!;
    
    [MaxLength(Constants.MaxLoginLength)]
    public string Login { get; set; } = null!;
    
    [MaxLength(Constants.MaxEmailLength)]
    public string Email { get; set; } = null!;
    
    [MaxLength(Constants.MaxPasswordHashLength)]
    public string PasswordHash { get; set; } = null!;
    public bool IsPremium { get; set; }
    public int PremiumPlan { get; set; }
    public DateTime PremiumCreateAt { get; set; }
    public DateTime PremiumExpiresAt { get; set; }
}
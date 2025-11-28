
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TFinanceWeb.Api.Models;

namespace TFinanceWeb.Api.Data.Configurations;

public class UserConfiguration
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.HasKey(u => u.UserId);
        
        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(u => u.Login)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.HasIndex(u => u.Email)
            .IsUnique();
        
        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);
        
        builder.Property(u => u.IsPremium)
            .HasDefaultValue(false);
        
        builder.Property(u => u.PremiumPlan)
            .HasDefaultValue(0);
        
        builder.Property(u => u.PremiumCreateAt)
            .HasDefaultValue(null);
        
        builder.Property(u => u.PremiumExpiresAt)
            .HasDefaultValue(null);
        
        
    }
}
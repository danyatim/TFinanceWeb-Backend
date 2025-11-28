namespace TFinanceWeb.Api.Utils;

public static class Constants
{
    //User auth related constants
    public const int MaxUsernameLength = 30;
    public const int MinUsernameLength = 3;
    public const int MaxPasswordHashLength = 50;
    public const int MinPasswordLength = 8;
    public const int MaxEmailLength = 50;
    public const int MinEmailLength = 5;
    public const int MaxLoginLength = 25;
    public const int MinLoginLength = 5;
    
    //Transaction related constants
    public const int MaxDescriptionLength = 255;
    public const int MinAmountValue = 0;
    public const int MaxAmountValue = 1000000000;
    public const int MaxBankAccountsPerUser = 3;
    public const int MaxBankAccountsPerUserPremium = 12;
}

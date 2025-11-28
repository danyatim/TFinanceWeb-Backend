namespace TFinanceWeb.Api.Utils;

public static class ValidateLogin
{
    public static (bool isValid, string errorMessage) Login(
        string login,
        string password
        )
    {
        if (string.IsNullOrWhiteSpace(login))
            return (false, "Логин обязателен");
        
        if (string.IsNullOrWhiteSpace(password))
            return (false, "Пароль обязателен");
        
        return (true, string.Empty);
    }
}
namespace TFinanceWeb.Api.Utils;

public static class ValidateRegister
{
    public static (bool isValid, string errorMessage) ValidateRegisterRequest(
        string login,
        string username,
        string email,
        string password,
        string confirmPassword
        )
    {
        //Валидация Email
        if (string.IsNullOrWhiteSpace(email))
            return (false, "Email обязателен");
        
        if (!ValidateEmail(email))
            return (false, "Неверный формат email.");
        
        
        //Валидация Логина
        if (login.Length < 5)
            return (false, "Логин должен содержать не менее 5 символов");
        
        if (string.IsNullOrWhiteSpace(login))
            return (false, "Логин обязателен");
        
        
        //Валидация Имени пользователя
        if (string.IsNullOrWhiteSpace(username))
            return (false, "Имя пользователя обязательно");
        
        
        //Валидация Пароля
        if (string.IsNullOrWhiteSpace(password))
            return (false, "Пароль обязателен");
        
        if (password.Length < 8)
            return (false, "Пароль должен содержать не менее 8 символов");
        
        if (string.IsNullOrWhiteSpace(confirmPassword))
            return (false, "Подтверждение пароля обязательно");
        
        if (password != confirmPassword)
            return (false, "Пароли не совпадают");
        
        
        return (true, string.Empty);
    }

    private static bool ValidateEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
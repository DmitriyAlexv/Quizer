namespace Quizer.Common;

/// <summary>
/// Исключение, возникающее при неудачной авторизации пользователя
/// (неверный email или пароль).
/// </summary>
public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message)
        : base(message)
    {
    }
}

namespace Quizer.Common;

/// <summary>
/// Исключение, возникающее при конфликте с текущим состоянием ресурса
/// (например, при попытке зарегистрировать пользователя с уже существующим email).
/// </summary>
public class ConflictException : Exception
{
    public ConflictException(string message)
        : base(message)
    {
    }
}

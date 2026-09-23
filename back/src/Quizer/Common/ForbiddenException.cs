namespace Quizer.Common;

/// <summary>
/// Исключение, возникающее при попытке выполнить операцию, на которую у пользователя нет прав.
/// </summary>
public class ForbiddenException : Exception
{
    public ForbiddenException(string message)
        : base(message)
    {
    }
}

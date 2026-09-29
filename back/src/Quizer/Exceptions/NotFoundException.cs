namespace Quizer.Exceptions;

/// <summary>
/// Исключение, возникающее при попытке обратиться к несуществующему ресурсу.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message)
        : base(message)
    {
    }
}

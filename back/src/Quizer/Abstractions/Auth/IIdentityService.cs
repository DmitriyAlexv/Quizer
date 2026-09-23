namespace Quizer.Abstractions.Auth;

/// <summary>
/// Сервис для работы с Identity (регистрация и авторизация пользователей).
/// </summary>
public interface IIdentityService
{
    /// <summary>
    /// Регистрирует нового пользователя и возвращает его идентификатор.
    /// </summary>
    Task<Guid> RegisterAsync(string email, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Авторизует пользователя и возвращает JWT-токен.
    /// </summary>
    Task<string> AuthorizeAsync(string email, string password, CancellationToken cancellationToken = default);
}

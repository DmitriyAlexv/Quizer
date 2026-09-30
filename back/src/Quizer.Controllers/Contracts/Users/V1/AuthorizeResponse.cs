namespace Quizer.Controllers.Contracts.Users.V1;

/// <summary>
/// Ответ на авторизацию пользователя.
/// </summary>
/// <param name="Token">JWT-токен для аутентификации пользователя.</param>
public record AuthorizeResponse(string Token);

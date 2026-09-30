namespace Quizer.Controllers.Contracts.Users.V1;

/// <summary>
/// Запрос на авторизацию пользователя.
/// </summary>
/// <param name="Email">Электронная почта пользователя.</param>
/// <param name="Password">Пароль пользователя.</param>
public record AuthorizeRequest(string Email, string Password);

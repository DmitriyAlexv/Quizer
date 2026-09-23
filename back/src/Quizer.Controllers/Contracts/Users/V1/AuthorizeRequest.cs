namespace Quizer.Controllers.Contracts.Users.V1;

/// <summary>
/// Запрос на авторизацию пользователя.
/// </summary>
public record AuthorizeRequest(string Email, string Password);

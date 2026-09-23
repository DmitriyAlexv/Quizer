namespace Quizer.Controllers.Contracts.Users.V1;

/// <summary>
/// Запрос на регистрацию пользователя.
/// </summary>
public record RegisterRequest(string Email, string Password, string Name);

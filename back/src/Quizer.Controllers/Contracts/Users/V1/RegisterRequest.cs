namespace Quizer.Controllers.Contracts.Users.V1;

/// <summary>
/// Запрос на регистрацию пользователя.
/// </summary>
/// <param name="Email">Электронная почта пользователя.</param>
/// <param name="Password">Пароль пользователя.</param>
/// <param name="Name">Имя пользователя.</param>
public record RegisterRequest(string Email, string Password, string Name);

namespace Quizer.Controllers.Contracts.Quizzes.V1;

/// <summary>
/// Запрос на создание попытки прохождения квиза.
/// </summary>
public record CreateAttemptRequest(Guid UserId);

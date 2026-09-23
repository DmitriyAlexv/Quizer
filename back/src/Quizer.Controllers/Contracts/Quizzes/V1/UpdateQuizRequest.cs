namespace Quizer.Controllers.Contracts.Quizzes.V1;

/// <summary>
/// Запрос на обновление квиза.
/// </summary>
public record UpdateQuizRequest(string Title, string Description);

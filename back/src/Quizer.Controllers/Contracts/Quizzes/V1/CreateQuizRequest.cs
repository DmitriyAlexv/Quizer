namespace Quizer.Controllers.Contracts.Quizzes.V1;

/// <summary>
/// Запрос на создание квиза.
/// </summary>
public record CreateQuizRequest(string Title, string Description, Guid OwnerId);

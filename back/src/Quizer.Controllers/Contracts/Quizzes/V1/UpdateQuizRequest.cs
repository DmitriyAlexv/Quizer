namespace Quizer.Controllers.Contracts.Quizzes.V1;

/// <summary>
/// Запрос на обновление квиза.
/// </summary>
/// <param name="Title">Название квиза.</param>
/// <param name="Description">Описание квиза.</param>
public record UpdateQuizRequest(string Title, string Description);

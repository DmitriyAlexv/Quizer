namespace Quizer.Controllers.Contracts.Quizzes.V1;

/// <summary>
/// Запрос на создание квиза.
/// </summary>
/// <param name="Title">Название квиза.</param>
/// <param name="Description">Описание квиза.</param>
public record CreateQuizRequest(string Title, string Description);

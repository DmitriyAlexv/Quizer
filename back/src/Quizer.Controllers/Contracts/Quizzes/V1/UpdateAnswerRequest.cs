namespace Quizer.Controllers.Contracts.Quizzes.V1;

/// <summary>
/// Запрос на обновление варианта ответа.
/// </summary>
public record UpdateAnswerRequest(string Text, bool IsCorrect);

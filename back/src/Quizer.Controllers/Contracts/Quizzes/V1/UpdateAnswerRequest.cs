namespace Quizer.Controllers.Contracts.Quizzes.V1;

/// <summary>
/// Запрос на обновление варианта ответа.
/// </summary>
/// <param name="Text">Текст варианта ответа.</param>
/// <param name="IsCorrect">Признак того, является ли вариант ответа правильным.</param>
public record UpdateAnswerRequest(string Text, bool IsCorrect);

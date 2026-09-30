namespace Quizer.Controllers.Contracts.Quizzes.V1;

/// <summary>
/// Запрос на добавление варианта ответа к вопросу.
/// </summary>
/// <param name="Text">Текст варианта ответа.</param>
/// <param name="IsCorrect">Признак того, является ли вариант ответа правильным.</param>
public record AddAnswerRequest(string Text, bool IsCorrect);

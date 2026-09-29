namespace Quizer.Controllers.Contracts.Quizzes.V1;

/// <summary>
/// Ответ с данными варианта ответа.
/// </summary>
/// <param name="Id">Идентификатор варианта ответа.</param>
/// <param name="Text">Текст варианта ответа.</param>
/// <param name="IsCorrect">Признак того, является ли вариант ответа правильным.</param>
public record AnswerResponse(Guid Id, string Text, bool IsCorrect);

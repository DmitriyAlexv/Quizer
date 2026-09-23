namespace Quizer.Controllers.Contracts.Quizzes.V1;

/// <summary>
/// Ответ с данными варианта ответа.
/// </summary>
public record AnswerResponse(Guid Id, string Text, bool IsCorrect);

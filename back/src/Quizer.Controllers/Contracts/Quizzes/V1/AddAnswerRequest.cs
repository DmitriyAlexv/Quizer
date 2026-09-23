namespace Quizer.Controllers.Contracts.Quizzes.V1;

/// <summary>
/// Запрос на добавление варианта ответа к вопросу.
/// </summary>
public record AddAnswerRequest(string Text, bool IsCorrect);

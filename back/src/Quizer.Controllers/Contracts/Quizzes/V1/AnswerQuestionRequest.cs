namespace Quizer.Controllers.Contracts.Quizzes.V1;

/// <summary>
/// Запрос на ответ на вопрос квиза.
/// </summary>
public record AnswerQuestionRequest(string? TextAnswer, IEnumerable<Guid>? SelectedAnswerIds);

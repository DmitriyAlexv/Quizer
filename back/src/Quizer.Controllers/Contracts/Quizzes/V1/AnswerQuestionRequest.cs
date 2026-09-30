namespace Quizer.Controllers.Contracts.Quizzes.V1;

/// <summary>
/// Запрос на ответ на вопрос квиза.
/// </summary>
/// <param name="TextAnswer">Текстовый ответ пользователя (для вопросов с открытым ответом).</param>
/// <param name="SelectedAnswerIds">Список идентификаторов выбранных вариантов ответа (для вопросов с выбором).</param>
public record AnswerQuestionRequest(string? TextAnswer, IEnumerable<Guid>? SelectedAnswerIds);

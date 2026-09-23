using Quizer.QuizAggregate.Enums;

namespace Quizer.Controllers.Contracts.Quizzes.V1;

/// <summary>
/// Запрос на обновление вопроса квиза.
/// </summary>
public record UpdateQuestionRequest(string Text, QuestionType Type, int Order, int Points);

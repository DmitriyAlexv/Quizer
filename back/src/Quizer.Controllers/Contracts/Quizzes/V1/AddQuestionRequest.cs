using Quizer.QuizAggregate;
using Quizer.QuizAggregate.Enums;

namespace Quizer.Controllers.Contracts.Quizzes.V1;

/// <summary>
/// Запрос на добавление вопроса в квиз.
/// </summary>
public record AddQuestionRequest(string Text, QuestionType Type, int Order, int Points);

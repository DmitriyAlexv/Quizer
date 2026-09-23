using Quizer.QuizAggregate.Enums;

namespace Quizer.Controllers.Contracts.Quizzes.V1;

/// <summary>
/// Ответ с данными вопроса.
/// </summary>
public record QuestionResponse(
    Guid Id,
    string Text,
    QuestionType Type,
    int Order,
    int Points,
    IReadOnlyCollection<AnswerResponse> Answers);

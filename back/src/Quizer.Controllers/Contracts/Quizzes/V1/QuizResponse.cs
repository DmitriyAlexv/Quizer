using Quizer.QuizAggregate;
using Quizer.QuizAggregate.Enums;

namespace Quizer.Controllers.Contracts.Quizzes.V1;

/// <summary>
/// Ответ с данными квиза.
/// </summary>
public record QuizResponse(
    Guid Id,
    string Title,
    string Description,
    Guid OwnerId,
    QuizStatus Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
